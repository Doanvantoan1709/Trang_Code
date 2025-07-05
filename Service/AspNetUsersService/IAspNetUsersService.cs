using BaseProject.Model.Entities;
using BaseProject.Service.AspNetUsersService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using Service.Common;

namespace BaseProject.Service.AspNetUsersService
{
    public interface IAspNetUsersService : IService<AppUser>
    {
        Task<PagedList<UserDto>> GetData(AspNetUsersSearch search, UserDto userDto = null);

        Task<AspNetUsersDto> GetDto(Guid id);

        Task<List<AppUser>> GetUserByCanBoIds(List<Guid> canboIds);

        Task<AppUser> GetUserByCanBoId(Guid? canboId);
        Task<List<AppUser>> GetUserByIdDonVi(Guid IdDonVi);
        Task<List<AppUser>> GetUserByIdDonViAndIdRole(Guid IdDonVi, Guid IdRole);
    }
}
