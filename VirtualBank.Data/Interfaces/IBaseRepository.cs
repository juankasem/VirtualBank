using System;
using System.Linq;
using System.Linq.Expressions;

namespace VirtualBank.Data.Interfaces
{
    public interface IBaseRepository<T>
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Save();
    }
}
