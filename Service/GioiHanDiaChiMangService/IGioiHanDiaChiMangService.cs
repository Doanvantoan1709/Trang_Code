using BaseProject.Model.Entities;
using BaseProject.Service.GioiHanDiaChiMangService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using BaseProject.Model.Entities;
using Service.Common;

namespace BaseProject.Service.GioiHanDiaChiMangService
{
    public interface IGioiHanDiaChiMangService : IService<GioiHanDiaChiMang>
    {
        Task<PagedList<GioiHanDiaChiMangDto>> GetData(GioiHanDiaChiMangSearch search);
        Task<GioiHanDiaChiMangDto?> GetDto(Guid id);
    }
}
