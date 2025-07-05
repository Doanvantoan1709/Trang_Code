using BaseProject.Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common
{
    public interface IService<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid? id);
        Task<T> GetByIdOrThrowAsync(Guid? guid);
        Task CreateAsync(T entity);
        Task CreateAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        IQueryable<T> GetQueryable();
        IQueryable<T> GetQueryableWithTracking();
        Task DeleteAsync(IEnumerable<T> entities);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        void Delete(Expression<Func<T, bool>> filter);
        Task<List<DropdownOption>> GetDropdownOptions<TField, TValue>(Expression<Func<T, TField>> displayField, Expression<Func<T, TValue>> valueField, TValue? selected = default);
        Task DeleteRange(Expression<Func<T, bool>> expression);
        void DeleteRange(IEnumerable<T> entities);
        Task InsertRange(List<T> listObj);
    }
}
