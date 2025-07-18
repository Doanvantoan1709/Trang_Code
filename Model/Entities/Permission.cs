using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Model.Entities
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public int AccountId { get; set; }
        public int FunctionId { get; set; }
        public int IsView { get; set; } 
        public int IsInsert { get; set; } 
        public int IsUpdate { get; set; }
        public int IsDelete { get; set; }
    }
}
