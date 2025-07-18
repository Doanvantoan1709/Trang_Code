using BaseProject.Model.Entities;
using BaseProject.Repository.AccountRepository;
using BaseProject.Service.AccountService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace BaseProjectNetCore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class Account2Controller : ControllerBase
    {
        private readonly IAccountService _accountService;

        public Account2Controller(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]AccountLoginRequestData requestData)
        {
            try
            {
                var res = await _accountService.Account_Login(requestData);
                return Ok(res);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody]AccountSignupRequestData requestData)
        {
            try
            {
                var res = await _accountService.Account_SignUp(new Account{
                   Username = requestData.Username,
                   Password = requestData.Password,
                   Fullname = requestData.Fullname,
                   Address = requestData.Address
                });
                return Ok(res);
            }catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPut("Change_information")]
        public async Task<IActionResult> ChangeInformation(Account requestData)
        {
            try
            {
                var res = await _accountService.Account_ChangeInfo(requestData);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPut("Change_password")]
        public async Task<IActionResult> ChangePassword(AccountRequestChangePass requestData)
        {
            try
            {
                var res = await _accountService.Account_ChangePass(requestData);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
