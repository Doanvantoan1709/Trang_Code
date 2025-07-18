using BaseProject.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Service.AccountService
{
    public interface IAccountService
    {
        Task<AccountLoginResponseData> Account_Login(AccountLoginRequestData requestData);
        Task<AcessRefresh_Token> Account_ARToken(AcessRefresh_Token tokenModel);
        Task<ReturnData> Account_SignUp(Account requestData);
        Task<ReturnData> Account_ChangeInfo(Account requestData);
        Task<ReturnData> Account_ChangePass(AccountRequestChangePass requestData);
    }
}
