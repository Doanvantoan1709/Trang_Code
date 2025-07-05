using BaseProject.Model.Entities;
using BaseProject.Service.AppUserService.Dto;
using BaseProject.Service.AppUserService.ViewModels;
using BaseProject.Service.Dto;
using Service.Common;

namespace BaseProject.Service.AppUserService
{
    public interface IAppUserService : IService<AppUser>
    {
        Task<LoginResponseDto> LoginUser(string email, string password);

        Task<string> ResetPassword(string email, string baseUri);

        Task<AppUserDto> ChangePassword(Guid? id, string oldPassword, string newPassword, string confirmPassword);

        Task<LoginResponseDto> RefreshToken(string refreshToken);

        Task<AppUserDto> CheckLogin(Guid? id);

        Task LogoutUser();

        Task<AppUser?> GetByUserName(string UserName);

        Task<UserInfoDto> GetInfo(Guid? id);

        Task<AppUserDto> RegisterUser(RegisterViewModel model);
        Task<List<DropdownOption>> GetDropDownUser(Guid? id);
    }
}
