using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.OperationRepository
{
    public class OperationRepository : Repository<Operation>, IOperationRepository
    {
        public OperationRepository(DbContext context) : base(context)
        {
        }
    }
}
