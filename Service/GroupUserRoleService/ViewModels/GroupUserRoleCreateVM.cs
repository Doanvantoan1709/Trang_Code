using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace BaseProject.Service.GroupUserRoleService.ViewModels
{
    public class GroupUserRoleCreateVM
    {
        [Required]
		public Guid GroupUserId {get; set; }
		public List<Guid>? RoleId {get; set; }
    }
}
