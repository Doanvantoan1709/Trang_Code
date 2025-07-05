using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.User_GroupUserRepository
{
    public class User_GroupUserRepository : Repository<User_GroupUser>, IUser_GroupUserRepository
    {
        public User_GroupUserRepository(DbContext context) : base(context)
        {
        }
    }
}
