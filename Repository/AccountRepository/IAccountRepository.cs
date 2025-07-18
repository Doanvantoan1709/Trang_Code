using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseProject.Model.Entities;

namespace BaseProject.Repository.AccountRepository
{
    public interface IAccountRepository
    {
        Task<Account> Account_Login(Account requestData);
        Task<int> Account_UpdateRefreshToken(AccUpdateRefreshTokenRequestData requestData);
        Task<Account> Account_GetInfoByUsername(string username);
        Task<Account> Account_GetInfoById(int accountId);
        Task Account_SignUp(Account requestData);
        Task<int> Account_ChangePassword(Account requestData);
        Task<int> Account_ChangeInfo(Account requestData);

    }
}
