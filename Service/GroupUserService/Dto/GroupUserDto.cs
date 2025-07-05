using BaseProject.Model.Entities;
using BaseProject.Model.Entities;

namespace BaseProject.Service.GroupUserService.Dto
{
    public class GroupUserDto : GroupUser
    {
        public List<Guid>? RoleIds { get; set; }
        public List<string>? RoleNames { get; set; }
    }
}
