using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.AspNetUsersRepository
{
    public class AspNetUsersRepository : Repository<AppUser>, IAspNetUsersRepository
    {
        public AspNetUsersRepository(DbContext context) : base(context)
        {
        }
    }
}
