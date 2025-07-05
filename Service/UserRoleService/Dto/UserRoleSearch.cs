using BaseProject.Service.Dto;


namespace BaseProject.Service.UserRoleService.Dto
{
    public class UserRoleSearch : SearchBase
    {
        public string? UserId {get; set; }
		public string? RoleId {get; set; }
		public string? CreatedId {get; set; }
		public string? UpdatedId {get; set; }
    }
}
