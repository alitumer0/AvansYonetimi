using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _repository.FindAsync(expression);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _repository.AddRangeAsync(entities);
            return entities;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _repository.UpdateAsync(entity);
        }

        public virtual async Task RemoveAsync(T entity)
        {
            await _repository.RemoveAsync(entity);
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            await _repository.RemoveRangeAsync(entities);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _repository.AnyAsync(expression);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression = null)
        {
            return await _repository.CountAsync(expression);
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
} 