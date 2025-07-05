using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Service.FileManagerService.Dto
{
    public class FilePermissionDto
    {
        public bool Create { get; set; }
        public bool Upload { get; set; }
        public bool Move { get; set; }
        public bool Copy { get; set; }
        public bool Rename { get; set; }
        public bool View { get; set; }
        public bool Download { get; set; }
        public bool Delete { get; set; }
        public bool Share { get; set; }
    }
}
