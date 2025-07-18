using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{
    public class UserSession
    {
        public int AccountId { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiredTime { get; set; }
    }
}
