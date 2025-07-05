using BaseProject.Model.Entities;
using BaseProject.Service.User_GroupUserService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using Service.Common;

namespace BaseProject.Service.User_GroupUserService
{
    public interface IUser_GroupUserService : IService<User_GroupUser>
    {
        Task<PagedList<User_GroupUserDto>> GetData(User_GroupUserSearch search);
        Task<User_GroupUserDto?> GetDto(Guid id);
    }
}
