using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Common;

namespace BaseProject.Repository.AppUserRepository
{
    public class AppUserRepository : Repository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(DbContext context) : base(context)
        {

        }
    }
}
