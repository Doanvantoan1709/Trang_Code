using BaseProject.Service.Common;
using BaseProject.Service.Dto;

namespace BaseProject.Service.RoleOperationService.Dto
{
    public class RoleOperationSearch : SearchBase
    {
        public string? CreatedId {get; set; }
		public string? UpdatedId {get; set; }
		public int RoleId {get; set; }
		public int IsAccess {get; set; }
		public long OperationId {get; set; }
    }
}
