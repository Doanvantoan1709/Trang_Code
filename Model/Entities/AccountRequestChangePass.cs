using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{
    public class AccountRequestChangePass
    {
        public int AccountId { get; set; }
        public string? Password { get; set; }
    }
}
