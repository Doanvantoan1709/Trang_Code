using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{
    public class AccountLoginResponseData : ReturnData
    {
        public int AccountId { get; set; }
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public string? Address { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiredTime { get; set; }
    }
}
