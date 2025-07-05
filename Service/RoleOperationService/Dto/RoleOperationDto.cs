using BaseProject.Model.Entities;
using BaseProject.Service.OperationService.Dto;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.Service.RoleOperationService.Dto
{
    public class RoleOperationDto : RoleOperation
    {
    }

    public class RoleOperationViewModel
    {
        public string? RoleName { get; set; }
        public Guid RoleId { get; set; }
        public bool IsAccess { get; set; }
        public Guid OperationId { get; set; }
       
        public string? OperationName { get; set; }
    }
}
