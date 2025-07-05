using BaseProject.Service.OperationService.Dto;

namespace BaseProject.Service.ModuleService.Dto
{
    public class ModuleGroup
    {
        public Guid ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public List<OperationDto>? Operations { get; set; }
        public List<string>? SelectedCodes { get; set; }
    }
}
