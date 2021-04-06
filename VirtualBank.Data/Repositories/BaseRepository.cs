using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T: class
    {
        private readonly VirtualBankDbContext _dbContext;

        public BaseRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }


        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

    }
}
