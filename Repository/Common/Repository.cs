using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Common
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext _entities;
        protected readonly DbSet<T> _dbset;

        public Repository(DbContext context)
        {
            _entities = context;
            _dbset = context.Set<T>();
        }


        public virtual DbSet<T> DBSet()
        {
            return _dbset;
        }

        public async Task<T?> GetByIdAsync(Guid? id)
        {
            return await _dbset.FindAsync(id);
        }
        public virtual IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable<T>();
        }
        public virtual IQueryable<T> GetQueryable()
        {
            return _dbset.AsNoTracking().AsQueryable();
        }
        public virtual IQueryable<T> GetQueryableWithTracking()
        {
            return _dbset.AsQueryable();
        }
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return GetQueryable().Where(predicate);
        }

        public virtual T Add(T entity)
        {
            return _dbset.Add(entity).Entity;
        }

        public virtual T Delete(T entity)
        {
            return _dbset.Remove(entity).Entity;
        }

        public virtual void Update(T entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _entities.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual async Task SaveAsync()
        {
            await _entities.SaveChangesAsync();
        }



        public void CreateRange(IEnumerable<T> entities)
        {
            _entities.Set<T>().AddRange(entities);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbset.AnyAsync(predicate);
        }

        public void Delete(Expression<Func<T, bool>> filter)
        {
            var entities = _dbset.Where(filter);
            _dbset.RemoveRange(entities);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbset.RemoveRange(entities);
        }


        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = _dbset.Where(predicate).AsEnumerable();
            return query;
        }

        public async Task InsertRange(List<T> entities)
        {
            if (entities != null && entities.Any())
            {
                await _entities.Set<T>().AddRangeAsync(entities);
                await SaveAsync();
            }
        }
    }
}
