using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.RoleOperationRepository
{
    public class RoleOperationRepository : Repository<RoleOperation>, IRoleOperationRepository
    {
        public RoleOperationRepository(DbContext context) : base(context)
        {
        }
    }
}
