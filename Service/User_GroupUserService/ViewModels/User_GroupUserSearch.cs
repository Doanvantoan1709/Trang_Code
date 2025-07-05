using BaseProject.Service.Dto;

namespace BaseProject.Service.User_GroupUserService.Dto
{
    public class User_GroupUserSearch : SearchBase
    {
        public Guid? UserId {get; set; }
		public Guid? GroupUserId {get; set; }
    }
}
