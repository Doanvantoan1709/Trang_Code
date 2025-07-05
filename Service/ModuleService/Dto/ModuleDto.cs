using BaseProject.Model.Entities;
using BaseProject.Service.OperationService.Dto;

namespace BaseProject.Service.ModuleService.Dto
{
    public class ModuleDto : Module
    {
        public string TrangThaiHienThi { get; set; }
        public List<Operation> ListOperation { get; set; }
        public string? DuongDanIcon { get; set; }
    }
}
