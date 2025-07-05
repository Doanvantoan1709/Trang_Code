using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.ModuleRepository
{
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        public ModuleRepository(DbContext context) : base(context)
        {
        }
    }
}
