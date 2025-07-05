using BaseProject.Model.Entities;
using BaseProject.Service.GroupUserService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using Service.Common;
using BaseProject.Model.Entities;

namespace BaseProject.Service.GroupUserService
{
    public interface IGroupUserService : IService<GroupUser>
    {
        Task<PagedList<GroupUserDto>> GetData(GroupUserSearch search);
        Task<GroupUserDto?> GetDto(Guid id);
    }
}
