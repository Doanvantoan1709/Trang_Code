using BaseProject.Model.Entities;
using BaseProject.Service.GroupUserRoleService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using BaseProject.Model.Entities;
using Service.Common;

namespace BaseProject.Service.GroupUserRoleService
{
    public interface IGroupUserRoleService : IService<GroupUserRole>
    {
        Task<PagedList<GroupUserRoleDto>> GetData(GroupUserRoleSearch search);
        Task<GroupUserRoleDto?> GetDto(Guid id);
    }
}
