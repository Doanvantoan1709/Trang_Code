using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.DepartmentRepository
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(DbContext context) : base(context)
        {
        }
    }
}
