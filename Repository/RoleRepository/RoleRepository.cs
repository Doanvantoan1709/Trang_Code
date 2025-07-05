using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;
using BaseProject.Model.Entities;


namespace BaseProject.Repository.RoleRepository
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(DbContext context) : base(context)
        {
        }
    }
}
