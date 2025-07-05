using BaseProject.Service.Dto;
using Microsoft.EntityFrameworkCore;
using Repository.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public Service(IRepository<T> repository)   
        {
            _repository = repository;
        }

        public async Task<T?> GetByIdAsync(Guid? guid)
        {
            return await _repository.GetByIdAsync(guid);
        }

        public async Task<T> GetByIdOrThrowAsync(Guid? guid)
        {
            var entity = await _repository.GetByIdAsync(guid);
            if (entity == null)
            {
                string entityName = typeof(T).Name;
                var displayNameAttribute = typeof(T).GetCustomAttribute<DisplayNameAttribute>();
                if (displayNameAttribute != null)
                {
                    entityName = displayNameAttribute.DisplayName;
                }
                throw new Exception($"Không tìm thấy {entityName} với Id {guid}");
            }
            return entity;
        }


        public virtual async Task CreateAsync(T entity)
        {
            _repository.Add(entity);
            await _repository.SaveAsync();
        }

        public virtual async Task CreateAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _repository.Add(entity);
            }
            await _repository.SaveAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _repository.SaveAsync();
        }

        public virtual async Task UpdateAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _repository.Update(entity);
            }
            await _repository.SaveAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _repository.Delete(entity);
            await _repository.SaveAsync();
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            if (entities != null && entities.Any())
            {
                _repository.DeleteRange(entities);
            }
        }

        public virtual async Task DeleteRange(Expression<Func<T, bool>> expression)
        {
            var entities = await _repository.GetQueryable().Where(expression).ToListAsync();
            if (entities != null && entities.Any())
            {
                _repository.DeleteRange(entities);
            }
        }

        public IQueryable<T> GetQueryable()
        {
            return _repository.GetQueryable();
        }

        public IQueryable<T> GetQueryableWithTracking()
        {
            return _repository.GetQueryableWithTracking();
        }
        public async Task DeleteAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _repository.Delete(entity);
            }
            await _repository.SaveAsync();
        }


        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _repository.GetQueryable().Where(predicate);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.GetQueryable().FirstOrDefaultAsync(predicate);
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return _repository.GetQueryable().Count(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.AnyAsync(predicate);
        }

        public void Delete(Expression<Func<T, bool>> filter)
        {
            //_repository.Delete(filter);
        }

        public async Task<List<DropdownOption>> GetDropdownOptions<TField, TValue>(Expression<Func<T, TField>> displayField, Expression<Func<T, TValue>> valueField, TValue? selected = default)
        {
            var displayFieldName = ((MemberExpression)displayField.Body).Member.Name;
            var valueFieldName = ((MemberExpression)valueField.Body).Member.Name;

#pragma warning disable CS8602
            var result = await _repository.GetQueryable()
                .Select(x => new DropdownOption
                {
                    Value = EF.Property<TValue>(x, valueFieldName).ToString(),
                    Label = EF.Property<TField>(x, displayFieldName).ToString(),
                    Selected = selected != null && selected.Equals(EF.Property<TValue>(x, valueFieldName))
                })
                .ToListAsync();
#pragma warning restore CS8602

            return result;
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _repository.FindBy(predicate);
        }


        public async Task InsertRange(List<T> listObj)
        {
            await _repository.InsertRange(listObj);
        }

    }
  
    
}
