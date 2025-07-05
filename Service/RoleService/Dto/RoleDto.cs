using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace BaseProject.Service.RoleService.Dto
{
    public class RoleDto : Role
    {
        public bool IsGanNguoi { get; set; }
        public string Type_txt { get; set; }
    }
}
