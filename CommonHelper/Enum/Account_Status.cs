using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper.Enum
{
    public enum Account_Status
    {
        CHANGE_INFOMATION_FALSE = -8,
        CHANGE_PASSWORD_FALSE = -7,
        ACCOUNT_IS_NOT_FOUND = -6,
        CHARACTERS_INVALID = -5,
        USER_IS_EXIST = -4,
        ACCOUNT_IS_NOT_EXIST = -3,
        ACCOUNT_SIGNUP_FALSE = -2,
        ACCOUNT_LOGIN_FALSE = -1,
        ACCOUNT_LOGIN_SUCCESS = 1,
        ACCOUNT_SIGNUP_SUCCESS = 2,
        CHANGE_PASSWORD_SUCCESS = 3,
        CHANGE_INFOMATION_SUCCESS = 3,
    }
}
