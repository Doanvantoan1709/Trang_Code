using BaseProject.Model.Entities;
using CommonHelper.Validate;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonHelper.Enum;
using CommonHelper.Sercurity;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


namespace BaseProject.Repository.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BaseProjectContext _dbContext;
        public AccountRepository(BaseProjectContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Account_ChangeInfo(Account requestData)
        {
            try
            {
                var user = await _dbContext.account.FindAsync(requestData.AccountId);
                if (user == null)
                {
                    throw new Exception($"Không tìm thấy tài khoản với AccountId = {requestData.AccountId}.");
                }

                if (!string.IsNullOrEmpty(requestData.Fullname))
                {
                    user.Fullname = requestData.Fullname;
                }

                if (!string.IsNullOrEmpty(requestData.Address))
                {
                    user.Address = requestData.Address;
                }

                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> Account_ChangePassword(Account requestData)
        {
            try
            {
                var user = await _dbContext.account.FindAsync(requestData.AccountId);
                if (user == null)
                {
                    throw new Exception($"Không tìm thấy tài khoản với AccountId = {requestData.AccountId}.");
                }

                if (!string.IsNullOrEmpty(requestData.Password))
                {
                    user.Password = requestData.Password;
                    _dbContext.Entry(user).Property(u => u.Password).IsModified = true;
                }

                return await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Account> Account_GetInfoById(int accountId)
        {
            try
            {
                return await _dbContext.account.FindAsync(accountId);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<Account> Account_GetInfoByUsername(string username)
        {
            try
            {
                return _dbContext.account.Where(x => x.Username == username).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Account> Account_Login(Account requestData)
        {
            try
            {
                var passwordHash = SecurityPassword.ComputeSalt256Hash(requestData.Password);
                return _dbContext.account.FirstOrDefault(x =>
                                    x.Username == requestData.Username &&
                                    x.Password == passwordHash);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Account_SignUp(Account requestData)
        {
            try
            {
                await _dbContext.account.AddAsync(requestData);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> Account_UpdateRefreshToken(AccUpdateRefreshTokenRequestData requestData)
        {
            try
            {
                var account = _dbContext.account.FirstOrDefault(x => x.AccountId == requestData.AccountId);
                if (account == null)
                {
                    throw new Exception("Account isn't exist");
                }
                else
                {
                    account.RefreshToken = requestData.RefreshToken;
                    account.ExpiredTime = requestData.ExpiredTime;
                    _dbContext.Update(account);
                    return await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
