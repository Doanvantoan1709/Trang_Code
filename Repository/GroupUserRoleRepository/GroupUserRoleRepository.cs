using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.GroupUserRoleRepository
{
    public class GroupUserRoleRepository : Repository<GroupUserRole>, IGroupUserRoleRepository
    {
        public GroupUserRoleRepository(DbContext context) : base(context)
        {
        }
    }
}
