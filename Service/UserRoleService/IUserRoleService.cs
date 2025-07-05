using BaseProject.Model.Entities;
using BaseProject.Service.UserRoleService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using BaseProject.Service.Dto;
using BaseProject.Service.UserRoleService.ViewModels;
using BaseProject.Model.Entities;
using Service.Common;

namespace BaseProject.Service.UserRoleService
{
    public interface IUserRoleService : IService<UserRole>
    {
        Task<PagedList<UserRoleDto>> GetData(UserRoleSearch search);

        Task<UserRoleDto> GetDto(Guid id);

        UserRole GetByUserAndRole(Guid userId, Guid roleId);

        List<UserRole> GetByUser(Guid userId);

        Task<UserRoleVM> GetUserRoleVM(Guid userId);
        List<string> GetListRoleCodeByUserId(Guid userId);
    }
}
