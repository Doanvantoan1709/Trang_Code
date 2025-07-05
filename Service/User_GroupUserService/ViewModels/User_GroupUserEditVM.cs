using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace BaseProject.Service.User_GroupUserService.ViewModels
{
    public class User_GroupUserEditVM : User_GroupUserCreateVM
    {
        public Guid? Id { get; set; }
    }
}
