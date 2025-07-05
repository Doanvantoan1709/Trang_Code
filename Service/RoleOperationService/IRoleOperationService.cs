using BaseProject.Model.Entities;
using BaseProject.Service.RoleOperationService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using Service.Common;

namespace BaseProject.Service.RoleOperationService
{
    public interface IRoleOperationService : IService<RoleOperation>
    {
        Task<PagedList<RoleOperationDto>> GetData(RoleOperationSearch search);

        Task<RoleOperationDto> GetDto(Guid id);

        List<RoleOperation> GetByRoleId(Guid RoleId);

        Task<List<RoleOperationViewModel>> GetOperationByRoleId(Guid? id);
    }
}
