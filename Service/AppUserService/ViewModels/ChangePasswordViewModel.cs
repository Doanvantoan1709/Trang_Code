using System.ComponentModel.DataAnnotations;

namespace BaseProject.Service.AppUserService.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
