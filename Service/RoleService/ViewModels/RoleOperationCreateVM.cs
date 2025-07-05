using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseProject.Service.RoleService.ViewModels
{
    public class RoleOperationMultiCreateVM
    {
        public Guid Id { get; set; }
        public List<Guid> ListOperation { get; set; }
    }
}
