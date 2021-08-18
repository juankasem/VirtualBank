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


        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _dbContext.Countries.Where(country => !country.Disabled)
                                             .AsNoTracking().ToListAsync();
        }


        public async Task<Country> FindByIdAsync(int id)
        {
            return await _dbContext.Countries.FirstOrDefaultAsync(c => c.Id == id && !c.Disabled);
                                                     
        }

        public async Task<Country> FindByIdWithCitiesAsync(int id)
        {
            return await _dbContext.Countries.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id && !c.Disabled);
        }


        public async Task<Country> AddAsync(Country country)
        {
            await _dbContext.Countries.AddAsync(country);

            return country;
        }


        public async Task<Country> UpdateAsync(Country country)
        {
            var existingCountry = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Id == country.Id && !c.Disabled);

            if (existingCountry != null)
            {
                _dbContext.Entry(existingCountry).State = EntityState.Detached;
            }

            _dbContext.Entry(country).State = EntityState.Modified;

            return country;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var country = await _dbContext.Countries.FindAsync(id);

            if (country != null)
            {
                country.Disabled = true;
                isDeleted = true;
            }
            
            return isDeleted;
        }

        public async Task<bool> CountryExistsAsync(int countryId) => await _dbContext.Countries.AnyAsync(c => c.Id == countryId);


        public async Task<bool> CountryNameExistsAsync(string countryName)
        {
            var exists = false;

            if(await _dbContext.Countries.FirstOrDefaultAsync(c => c.Name.Trim() == countryName.Trim()) != null)
            {
                exists = true;
            }

            return exists;
        }
    }
}
