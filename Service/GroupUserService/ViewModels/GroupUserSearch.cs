using BaseProject.Service.Dto;

namespace BaseProject.Service.GroupUserService.Dto
{
    public class GroupUserSearch : SearchBase
    {
        public string? Name {get; set; }
		public string? Code {get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
