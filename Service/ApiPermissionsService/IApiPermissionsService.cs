using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using BaseProject.Service.ApiPermissionsService.Dto;
using BaseProject.Service.ApiPermissionsService.ViewModels;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using Service.Common;

namespace BaseProject.Service.ApiPermissionsService
{
    public interface IApiPermissionsService : IService<ApiPermissions>
    {
        Task<PagedList<ApiPermissionsDto>> GetData(ApiPermissionsSearch search);
        Task<ApiPermissionsDto?> GetDto(Guid id);
        Task<List<string?>> GetApiPermistionOfUser(Guid? userId);
        Task<List<ApiPermissions>> GetByUserId(Guid? userId);
        Task<List<ApiPermissions>> GetByRoleId(Guid? roleId);
        Task Save(ApiPermissionsSaveVM model);
    }
}
