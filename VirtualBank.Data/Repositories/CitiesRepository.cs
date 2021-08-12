using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class CitiesRepository : ICitiesRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public CitiesRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _dbContext.Cities.Include(c => c.Country)
                                          .Where(city => !city.Disabled)
                                          .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<City>> GetByCountryIdAsync(int countryId)
        {
            return await _dbContext.Cities.Include(c => c.Country)
                                          .Where(city => city.CountryId == countryId && !city.Disabled)
                                          .AsNoTracking().ToListAsync();
        }


        public async Task<City> FindByIdAsync(int id)
        {
            return await _dbContext.Cities.Include(c => c.Country).FirstOrDefaultAsync(city => city.Id == id && !city.Disabled);                                                      
        }
       

        public async Task<City> AddAsync(City city)
        {
            await _dbContext.Cities.AddAsync(city);
            await SaveAsync();

            return city;
        }


        public async Task<City> UpdateAsync(City city)
        {
            var existingCity = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == city.Id && !city.Disabled);

            if (existingCity != null)
            {
                _dbContext.Entry(existingCity).State = EntityState.Detached;
            }

            _dbContext.Entry(city).State = EntityState.Modified;
            await SaveAsync();

            return city;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var city = await _dbContext.Cities.FindAsync(id);

            if (city != null)
            {
                city.Disabled = true;
                await SaveAsync();

                isDeleted = true;
            }

            return isDeleted;
        }


        public async Task<bool> CityExists(int cityId)
        {
            return await _dbContext.Cities.AnyAsync(c => c.Id == cityId);
        }


        public async Task<bool> CityNameExists(int countryId, string cityName)
        {
            return await _dbContext.Cities.AnyAsync(c => c.CountryId == countryId && c.Name == cityName);
        }


        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }   
    }
}
