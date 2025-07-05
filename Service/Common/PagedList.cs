using BaseProject.Service.Dto;
using Microsoft.EntityFrameworkCore;

namespace BaseProject.Service.Common
{
    public class PagedList<T>
    {

        public PagedList(List<T> items, int pageIndex, int pageSize, int totalCount)
        {
            Items = items;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
        public List<T> Items { get; set; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPage => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, SearchBase search)
        {
            var totalCount = await query.CountAsync();
            var items = await query.Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToListAsync();
            return new PagedList<T>(items, search.PageIndex, search.PageSize, totalCount);
        }
        public static Task<PagedList<T>> CreateAsync(IEnumerable<T> source, SearchBase search)
        {
            var count = source.Count();
            var items = source
                .Skip((search.PageIndex - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();

            return Task.FromResult(new PagedList<T>(items, count, search.PageIndex, search.PageSize));
        }
    }
}
