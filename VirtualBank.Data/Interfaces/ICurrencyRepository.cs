using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Data.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<Currency>> GetAllAsync();
        Task<Currency> FindByIdAsync(int id);
        Task<Currency> AddAsync(Currency urrency);
        Task<Currency> UpdateAsync(Currency currency);
        Task<bool> RemoveAsync(int id);
    }
}