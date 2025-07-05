using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;
using BaseProject.Model.Entities;


namespace BaseProject.Repository.GroupUserRepository
{
    public class GroupUserRepository : Repository<GroupUser>, IGroupUserRepository
    {
        public GroupUserRepository(DbContext context) : base(context)
        {
        }
    }
}
