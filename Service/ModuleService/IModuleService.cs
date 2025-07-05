using BaseProject.Model.Entities;
using BaseProject.Service.ModuleService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using BaseProject.Service.Dto;
using Service.Common;

namespace BaseProject.Service.ModuleService
{
    public interface IModuleService : IService<Module>
    {
        Task<PagedList<ModuleDto>> GetData(ModuleSearch search);

        Task<List<DropdownOption>> GetDropDown(string? selected);

        Task<ModuleDto> GetDto(Guid id);

        Task<List<ModuleGroup>> GetModuleGroupData(Guid roleId);
    }
}
