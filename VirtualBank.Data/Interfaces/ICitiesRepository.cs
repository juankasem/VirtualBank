using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface ICitiesRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<IEnumerable<City>> GetByCountryIdAsync(int countryId);
        Task<City> FindByIdAsync(int id);
        Task<City> AddAsync(City city);
        Task<City> UpdateAsync(City city);
        Task<bool> RemoveAsync(int id);
        Task<bool> CityExists(int cityId);
        Task<bool> CityNameExists(int countryId, string cityName);
        Task SaveAsync();
    }
}
