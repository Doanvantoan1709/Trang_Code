using BaseProject.Model.Entities;
using BaseProject.Service.DepartmentService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using BaseProject.Service.DepartmentService.ViewModels;
using BaseProject.Service.Dto;
using Service.Common;

namespace BaseProject.Service.DepartmentService
{
    public interface IDepartmentService : IService<Department>
    {
        Task<PagedList<DepartmentDto>> GetData(DepartmentSearch search);

        Task<DepartmentDto> GetDto(Guid id);

        List<DepartmentHierarchy> GetDepartmentHierarchy(bool isParent = false);
         
        Task<DepartmentDto> GetDetail(Guid id);

        Task<List<DropdownOption>> GetDropDown(string? selected);
        Task<List<DropdownOption>> GetDropDownByShortName(string? shortName);

        Task<List<DropdownOption>> GetDropRolesInDepartment(Guid? departmentId, Guid? userId);

        Task<List<DropdownOptionTree>> GetDropdownTreeOption(bool disabledParent = true);

        List<DepartmentVM> BuildDepartmentHierarchy();

        Task<List<DepartmentExport>> GetDepartmentExportData(string type);
        List<Guid> GetChildIds(List<Guid> ids);
        Task<List<DropdownOptionTree>> GetDropdownTreeOptionByUserDepartment(bool disabledParent = true, Guid? donViId = null);
        Task<List<DropdownOptionTree>> GetSubAndCurrentUnitDropdownTreeByUserDepartment(bool disabledParent = true, Guid? donViId = null);
        Task<List<DropdownOptionTree>> GetCodeDropdownTreeOption(bool disabledParent = true);
        Task<Guid> GetUserIdByRoleAndDepartment(Guid donViId, string maVaiTro);
        List<DepartmentHierarchy> GetDepartmentHierarchyV2(Guid? dvID);
        List<Department> getListDepartmentByListId(string litstDepartmentId);
    }
}
