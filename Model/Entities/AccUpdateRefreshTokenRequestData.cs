using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{
    public class AccUpdateRefreshTokenRequestData
    {
        public int AccountId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiredTime { get; set; }
    }
}
