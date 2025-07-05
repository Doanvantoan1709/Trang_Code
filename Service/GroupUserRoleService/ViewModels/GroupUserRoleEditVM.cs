using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace BaseProject.Service.GroupUserRoleService.ViewModels
{
    public class GroupUserRoleEditVM : GroupUserRoleCreateVM
    {
        public Guid? Id { get; set; }
    }
}
