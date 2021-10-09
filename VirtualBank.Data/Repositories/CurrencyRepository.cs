using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        public Task<Currency> AddAsync(Currency urrency)
        {
            throw new System.NotImplementedException();
        }

        public Task<Currency> FindByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Currency>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Currency> UpdateAsync(Currency currency)
        {
            throw new System.NotImplementedException();
        }
    }
}