using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected VirtualBankDbContext _dbContext;
        private DbSet<TEntity> _dbSet;

        public GenericRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }


        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }


        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return await _dbSet.Where(match).ToListAsync();
        }


        public async Task<IEnumerable<TEntity>> GetAllIncludeAsync(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> alllEntities = _dbSet;

            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                alllEntities = alllEntities.Include<TEntity, object>(includeProperty);
            }

            return await alllEntities.AsNoTracking().ToListAsync();
        }


        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }


        public async Task<TEntity> GetByIdIncludeAsync(int id,
            Expression<Func<TEntity, bool>> match,
            params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            if (includeExpressions.Any())
            {
                var set =  includeExpressions.Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>
                          (_dbSet, (current, expression) => current.Include(expression));

                return await set.FirstOrDefaultAsync(match);
            }

            return await GetByIdAsync(id);
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await  _dbContext.Set<TEntity>().AddAsync(entity);

            return entity;
        }


        public async Task<TEntity> UpdateAsync(TEntity entity, object key)
        {
            if (entity == null)
            {
                return null;
            }

            TEntity existingEntity =await _dbSet.FindAsync(key);

            if (existingEntity != null)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

            }

            return existingEntity;
        }

        public void DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
    }
}
