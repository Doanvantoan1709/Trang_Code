using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.FileManagerRepository
{
    public class FileManagerRepository : Repository<FileManager>, IFileManagerRepository
    {
        public FileManagerRepository(DbContext context) : base(context)
        {
        }
    }
}
