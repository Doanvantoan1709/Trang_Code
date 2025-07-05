using BaseProject.Service.Dto;

namespace BaseProject.Service.FileManagerService.Dto
{
    public class FileManagerSearch : SearchBase
    {
        public string? Name {get; set; }
		public Guid? ParentId {get; set; }
		public bool? IsDirectory {get; set; }
        public Guid UserID { get; set; }
        public Guid? UserDepartmentID { get; set; }
    }
}
