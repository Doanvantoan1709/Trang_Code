using BaseProject.Model.Entities;
using BaseProject.Repository.AccountRepository;
using CommonHelper.Enum;
using CommonHelper.Validate;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.IO;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using CommonHelper.Sercurity;
using Org.BouncyCastle.Asn1.Ocsp;
using DocumentFormat.OpenXml.Office2016.Excel;


namespace BaseProject.Service.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        public AccountService(IAccountRepository accountRepository, IConfiguration configuration, IDistributedCache cache)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<ReturnData> Account_SignUp(Account requestData)
        {
            try
            {
                var existing = await _accountRepository.Account_GetInfoByUsername(requestData.Username);
                //check user exist
                if (existing != null)
                {
                    return new ReturnData
                    {
                        ReturnCode = (int)Account_Status.USER_IS_EXIST,
                        ReturnMessage = "User is exist"
                    };
                }

                // check xss
                if (ValidateInput.CheckXSSInput(requestData.Username) == true ||
                    ValidateInput.CheckXSSInput(requestData.Password) == true ||
                    ValidateInput.CheckXSSInput(requestData.Fullname) == true ||
                    ValidateInput.CheckXSSInput(requestData.Address) == true)
                {
                    return new ReturnData
                    {
                        ReturnCode = (int)Account_Status.CHARACTERS_INVALID,
                        ReturnMessage = "Please enter valid characters",
                    };
                }

                var hashedPassword = SecurityPassword.ComputeSalt256Hash(requestData.Password);

                var new_account = new Account
                {
                    Username = requestData.Username,
                    Password = hashedPassword,
                    Fullname = requestData.Fullname,
                    Address = requestData.Address,
                    //IsAdmin = 0
                };

                await _accountRepository.Account_SignUp(new_account);
                return new ReturnData
                {
                    ReturnCode = (int)Account_Status.ACCOUNT_SIGNUP_SUCCESS,
                    ReturnMessage = "Sign up successful"
                };
            }
            catch (Exception ex)
            {
                return new ReturnData
                {
                    ReturnCode = (int)Account_Status.ACCOUNT_SIGNUP_FALSE,
                    ReturnMessage = "Sign up false"
                };
            }
        }

        public async Task<ReturnData> Account_ChangeInfo(Account requestData)
        {
            try
            {
                var account = await _accountRepository.Account_GetInfoById(requestData.AccountId);
                // check account null
                if (account == null)
                {
                    return new ReturnData
                    {
                        ReturnCode = (int)Account_Status.ACCOUNT_IS_NOT_FOUND,
                        ReturnMessage = "Account isn't found"
                    };
                }

                // check xss
                if (ValidateInput.CheckXSSInput(requestData.Fullname) == true ||
                    ValidateInput.CheckXSSInput(requestData.Address) == true)
                {
                    return new ReturnData
                    {
                        ReturnCode = (int)Account_Status.CHARACTERS_INVALID,
                        ReturnMessage = "Please enter valid characters",
                    };
                }

                var update_info = new Account
                {
                    AccountId = requestData.AccountId,
                    Fullname = requestData.Fullname,
                    Address = requestData.Address
                };

                await _accountRepository.Account_ChangeInfo(update_info);
                return new ReturnData
                {
                    ReturnCode = (int)Account_Status.CHANGE_INFOMATION_SUCCESS,
                    ReturnMessage = "Change infomation successful"
                };
            }
            catch(Exception ex)
            {
                return new ReturnData
                {
                    ReturnCode = (int)Account_Status.CHANGE_INFOMATION_FALSE,
                    ReturnMessage = "Change infomation false"
                };
            }
        }

        public async Task<ReturnData> Account_ChangePass(AccountRequestChangePass requestData)
        {
            try
            {
                var account = await _accountRepository.Account_GetInfoById(requestData.AccountId);
                // check account null
                if (account == null)
                {
                    return new ReturnData
                    {
                        ReturnCode = (int)Account_Status.ACCOUNT_IS_NOT_FOUND,
                        ReturnMessage = "Account isn't found"
                    };
                }

                // check xss
                if (ValidateInput.CheckXSSInput(requestData.Password) == true)
                {
                    return new ReturnData
                    {
                        ReturnCode = (int)Account_Status.CHARACTERS_INVALID,
                        ReturnMessage = "Please enter valid characters",
                    };
                }

                var update_pass = new Account
                {
                    AccountId = account.AccountId,
                    Password = account.Password = SecurityPassword.ComputeSalt256Hash(requestData.Password)
                };
                
                await _accountRepository.Account_ChangePassword(update_pass);
                return new ReturnData
                {
                    ReturnCode = (int)Account_Status.CHANGE_PASSWORD_SUCCESS,
                    ReturnMessage = "Change password successful"
                };
            }
            catch (Exception ex)
            {
                return new ReturnData
                {
                    ReturnCode = (int)Account_Status.CHANGE_PASSWORD_FALSE,
                    ReturnMessage = "Change password false"
                };
            }
        }

        public async Task<AccountLoginResponseData> Account_Login(AccountLoginRequestData requestData)
        {
            var return_data = new AccountLoginResponseData();
            try
            {
                // check xss
                if (ValidateInput.CheckXSSInput(requestData.Username) == true ||
                    ValidateInput.CheckXSSInput(requestData.Password) == true)
                {
                    return_data.ReturnCode = (int)Account_Status.CHARACTERS_INVALID;
                    return_data.ReturnMessage = "Please enter valid characters";
                    return return_data;
                }

                var acc = await _accountRepository.Account_Login(new Account
                {
                    Username = requestData.Username,
                    Password = requestData.Password
                });

                // check account exist
                if (acc == null)
                {
                    return_data.ReturnCode = (int)Account_Status.ACCOUNT_IS_NOT_EXIST;
                    return_data.ReturnMessage = "Account isn't exist";
                    return return_data;
                }

                // create claim
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.PrimarySid, acc.AccountId.ToString()),
                    new Claim(ClaimTypes.Name, acc.Username),
                    new Claim(ClaimTypes.Role, acc.IsAdmin.ToString()),
                };

                // create access token
                var jwt_securitytoken = CreateToken(authClaims);
                var access_token = new JwtSecurityTokenHandler().WriteToken(jwt_securitytoken);

                // create refresh token
                var refresh_token = GenerateRefreshToken();

                // take expired refresh token to config
                var expriredRefreshtoken = DateTime.Now.AddDays(Convert.ToInt32(_configuration["RefreshTokenValidityInDays"]));

                // save token to redis with life time equal expired token
                var token_redis = new UserSession
                {
                    AccountId = acc.AccountId,
                    Token = access_token,
                    ExpiredTime = jwt_securitytoken.ValidFrom
                };

                // save update refresh token to database
                var update_refreshtoken = await _accountRepository.Account_UpdateRefreshToken(new AccUpdateRefreshTokenRequestData
                {
                    AccountId = acc.AccountId,
                    RefreshToken = refresh_token,
                    ExpiredTime = expriredRefreshtoken
                });

                // save token to redis cache
                var key_cache = $"User_Session" + acc.AccountId;
                var cache_option = new DistributedCacheEntryOptions
                {
                    // life time of cache
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };

                await _cache.SetStringAsync(key_cache, Newtonsoft.Json.JsonConvert.SerializeObject(token_redis), cache_option);

                // return data
                return_data.ReturnCode = (int)Account_Status.ACCOUNT_LOGIN_SUCCESS;
                return_data.ReturnMessage = "Login successful";
                return_data.AccountId = acc.AccountId;
                return_data.Username = acc.Username;
                return_data.Fullname = acc.Fullname;
                return_data.Address = acc.Address;
                return_data.Token = access_token;
                return_data.RefreshToken = refresh_token;
                return_data.ExpiredTime = expriredRefreshtoken;
                return return_data;
            }
            catch (Exception ex)
            {
                return_data.ReturnCode = (int)Account_Status.ACCOUNT_LOGIN_FALSE;
                return_data.ReturnMessage = "Login false";
                return return_data;
            }
        }

        public async Task<AcessRefresh_Token> Account_ARToken(AcessRefresh_Token tokenModel)
        {
            try
            {
                string? access_token = tokenModel.AccessToken;
                string? refresh_token = tokenModel.RefreshToken;

                // decode access token
                var principal = GetPrincipalFromExpiredToken(access_token);
                if (principal != null)
                {
                    // take Name to Identity in funct GetPrincipalFromExpiredToken
                    string username = principal.Identity.Name;

                    var user = await _accountRepository.Account_GetInfoByUsername(username);
                    if (user != null || user.RefreshToken == refresh_token || user.ExpiredTime > DateTime.Now)
                    {
                        // create new access/refresh token 
                        var new_access_token = CreateToken(principal.Claims.ToList());
                        var new_refresh_token = GenerateRefreshToken();

                        // create new expired refresh token
                        var new_expired_refresh_token = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JWT:RefreshTokenValidityInDays"]));

                        // update refresh token and expired time
                        await _accountRepository.Account_UpdateRefreshToken(new AccUpdateRefreshTokenRequestData
                        {
                            AccountId = user.AccountId,
                            RefreshToken = new_refresh_token,
                            ExpiredTime = new_expired_refresh_token
                        });

                        return new AcessRefresh_Token
                        {
                            AccessToken = new JwtSecurityTokenHandler().WriteToken(new_access_token),
                            RefreshToken = new_refresh_token,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        /// Hàm tạo access token
        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;

        }

        /// Hàm tạo refresh token
        private static string GenerateRefreshToken()
        {
            var random_digit = new byte[32];
            using var rgn = RandomNumberGenerator.Create();
            rgn.GetBytes(random_digit);
            return Convert.ToBase64String(random_digit);
        }

        /// Hàm giải mã token dựa trên secret key
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var token_validation_parameter = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]))
            };

            var token_handler = new JwtSecurityTokenHandler();
            var principal = token_handler.ValidateToken(token, token_validation_parameter, out SecurityToken security_token);
            if (security_token is not JwtSecurityToken jwt_security_token || !jwt_security_token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid access token");
            }

            return principal;
        }
    }
}
