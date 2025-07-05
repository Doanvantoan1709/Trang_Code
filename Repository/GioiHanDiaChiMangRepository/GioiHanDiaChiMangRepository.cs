using Microsoft.EntityFrameworkCore;
using BaseProject.Model.Entities;
using Repository.Common;
using BaseProject.Model.Entities;


namespace BaseProject.Repository.GioiHanDiaChiMangRepository
{
    public class GioiHanDiaChiMangRepository : Repository<GioiHanDiaChiMang>, IGioiHanDiaChiMangRepository
    {
        public GioiHanDiaChiMangRepository(DbContext context) : base(context)
        {
        }
    }
}
    
