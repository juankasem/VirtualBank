using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface ICountriesRepository
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<Country> FindByIdAsync(int id);
        Task<Country> FindByIdWithCitiesAsync(int id);
        Task<Country> AddAsync(Country country);
        Task<Country> UpdateAsync(Country country);
        Task<bool> RemoveAsync(int id);
    }
}
