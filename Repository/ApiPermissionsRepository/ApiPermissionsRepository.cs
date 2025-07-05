using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.ApiPermissionsRepository
{
    public class ApiPermissionsRepository : Repository<ApiPermissions>, IApiPermissionsRepository
    {
        public ApiPermissionsRepository(DbContext context) : base(context)
        {
        }
    }
}
