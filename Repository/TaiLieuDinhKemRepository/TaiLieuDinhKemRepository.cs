using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using BaseProject.Model.Entities;
using Repository.Common;


namespace BaseProject.Repository.TaiLieuDinhKemRepository
{
    public class TaiLieuDinhKemRepository : Repository<TaiLieuDinhKem>, ITaiLieuDinhKemRepository
    {
        public TaiLieuDinhKemRepository(DbContext context) : base(context)
        {
        }
    }
}
