using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Address { get; set; }
        public string? Fullname { get; set; }
        public int IsAdmin { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiredTime { get; set; }
    }
}
