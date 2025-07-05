using BaseProject.Model.Entities;
using BaseProject.Service.TaiLieuDinhKemService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Common.Service;
using BaseProject.Model.Entities;
using Service.Common;

namespace BaseProject.Service.TaiLieuDinhKemService
{
    public interface ITaiLieuDinhKemService : IService<TaiLieuDinhKem>
    {
        Task<PagedList<TaiLieuDinhKemDto>> GetData(TaiLieuDinhKemSearch search);

        Task<TaiLieuDinhKemDto> GetDto(Guid id);

        Task<List<TaiLieuDinhKem>> GetByItemAsync(Guid itemId);

        Task<List<TaiLieuDinhKem>> GetByIdsAsync(string ids);
        Task<List<TaiLieuDinhKem>> GetByIdsAsync(List<Guid>? ids);

        Task UpdateItemIdAsync(List<Guid>? ids, Guid ItemId);

        Task<List<TaiLieuDinhKem>> GetKySo(List<Guid> Ids);

        Task<string> GetPathFromId(Guid id);

        Task<TaiLieuDinhKem> AddOrEditPath(string FilePath, Guid Id);

        Task<string> GetPathItem(Guid ItemId);
    }
}
