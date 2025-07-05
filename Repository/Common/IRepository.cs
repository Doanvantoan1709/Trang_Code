using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Common
{
    public interface IRepository<T> where T : class

    {
        Task<T?> GetByIdAsync(Guid? id);

        IEnumerable<T> GetAll();
        IQueryable<T> GetQueryable();
        IQueryable<T> GetQueryableWithTracking();
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        T Add(T entity);

        T Delete(T entity);

        void Update(T entity);

        Task SaveAsync();

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        void Delete(Expression<Func<T, bool>> filter);
        void DeleteRange(IEnumerable<T> entities);
        Task InsertRange(List<T> entities);
        void UpdateRange(IEnumerable<T> entities);
    }
}
