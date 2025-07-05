using BaseProject.Model.Entities;
using BaseProject.Service.OperationService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using Service.Common;

namespace BaseProject.Service.OperationService
{
    public interface IOperationService : IService<Operation>
    {
        Task<PagedList<OperationDto>> GetData(OperationSearch search);

        Task<OperationDto> GetDto(Guid id);

        Task<List<MenuDataDto>> GetListOperationOfUser(Guid userId);

        Task<List<ModuleMenuDTO>> GetListOperationOfRole(Guid roleId);

        Task<List<MenuDataDto>> GetListMenu(Guid userId, List<string> RoleCodes);
        
        Task<dynamic> GetOperationWithModuleByUrl(string url);
        
    }
}
