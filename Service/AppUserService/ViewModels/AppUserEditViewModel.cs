using System.ComponentModel.DataAnnotations;

namespace BaseProject.Service.AppUserService.ViewModels
{
    public class AppUserEditViewModel
    {
        public string? FamilyName { get; set; }
        public string? Name { get; set; }
        public string? GivenName { get; set; }
        public int Gender { get; set; }
    }
}
