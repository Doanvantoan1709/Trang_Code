using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;
using BaseProject.Model.Entities;


namespace BaseProject.Repository.FileSecurityRepository
{
    public class FileSecurityRepository : Repository<FileSecurity>, IFileSecurityRepository
    {
        public FileSecurityRepository(DbContext context) : base(context)
        {
        }
    }
}
