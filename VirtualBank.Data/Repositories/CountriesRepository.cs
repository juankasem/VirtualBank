using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public CountriesRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IEnumerable<Country>> GetAll()
        {
            return await _dbContext.Countries.Where(country => country.Disabled == false)
                                             .AsNoTracking().ToListAsync();
        }


        public async Task<Country> FindByIdAsync(int id)
        {
            return await _dbContext.Countries .FirstOrDefaultAsync(country => country.Id == id &&
                                                                              country.Disabled == false);
                                                     
        }

        public async Task<Country> FindByIdWithCitiesAsync(int id)
        {
            return await _dbContext.Countries.Include(c => c.Cities)
                                             .FirstOrDefaultAsync(country => country.Id == id &&
                                                                             country.Disabled == false);
        }


        public async Task<Country> AddAsync(Country country)
        {
            await _dbContext.Countries.AddAsync(country);
            await SaveAsync();

            return country;
        }


        public async Task<Country> UpdateAsync(Country country)
        {
            var existingCountry = await _dbContext.Countries.FirstOrDefaultAsync(country => country.Id == country.Id &&
                                                                                            country.Disabled == false);

            if (existingCountry is not null)
            {
                _dbContext.Entry(existingCountry).State = EntityState.Detached;
            }

            _dbContext.Entry(country).State = EntityState.Modified;
            await SaveAsync();

            return country;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var country = await _dbContext.Countries.FindAsync(id);

            if (country is not null)
            {
                country.Disabled = true;
                await SaveAsync();

                isDeleted = true;
            }

            return isDeleted;
        }


        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
