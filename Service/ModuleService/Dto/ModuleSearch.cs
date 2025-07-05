using BaseProject.Service.Dto;


namespace BaseProject.Service.ModuleService.Dto
{
    public class ModuleSearch : SearchBase
    {
		public bool? IsShow {get; set; }
		public string? Code {get; set; }
		public string? Name {get; set; }
    }
}
