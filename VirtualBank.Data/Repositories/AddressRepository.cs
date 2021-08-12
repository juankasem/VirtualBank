using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public AddressRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _dbContext.Addresses.Include(address => address.District)
                                             .Include(address => address.City)
                                             .Include(address => address.Country)
                                             .Where(address => !address.Disabled)
                                             .AsNoTracking().ToListAsync();
        }   

       
        public async Task<Address> FindByIdAsync(int id)
        {
            return await _dbContext.Addresses.Include(address => address.District)
                                             .Include(address => address.City)
                                             .Include(address => address.Country)
                                             .FirstOrDefaultAsync(address => !address.Disabled);
        }



        public async Task<Address> AddAsync(Address address, VirtualBankDbContext dbContext = null)
        {
            if (dbContext != null)
            {
                await dbContext.Addresses.AddAsync(address);
                await SaveAsync(dbContext);
            }
            else
            {
                await _dbContext.Addresses.AddAsync(address);
                await SaveAsync();
            }
           

            return address;
        }


        public async Task<Address> UpdateAsync(Address address, VirtualBankDbContext dbContext = null)
        {

            if (dbContext != null)
            {
                var existingAddress = await dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == address.Id && !a.Disabled);

                if (existingAddress != null)
                {
                    dbContext.Entry(existingAddress).State = EntityState.Detached;
                }

                dbContext.Entry(address).State = EntityState.Modified;
                await SaveAsync(dbContext);
            }

            else
            {
                var existingAddress = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == address.Id && !a.Disabled);

                if (existingAddress != null)
                {
                    _dbContext.Entry(existingAddress).State = EntityState.Detached;
                }

                _dbContext.Entry(address).State = EntityState.Modified;
                await SaveAsync();
            }
        

            return address;
        }


        public async Task<bool> RemoveAsync(int id, VirtualBankDbContext dbContext = null)
        {
            var isDeleted= false;

            if (dbContext != null)
            {
                var address = await dbContext.Addresses.FindAsync(id);

                if (address != null)
                {
                    address.Disabled = true;
                    await SaveAsync(dbContext);
                    isDeleted = true;
                }
            }
            else
            {
                var address = await _dbContext.Addresses.FindAsync(id);

                if (address != null)
                {
                    address.Disabled = true;
                    await SaveAsync();
                    isDeleted = true;
                }
            }
          
            return isDeleted;
        }


        public async Task SaveAsync(VirtualBankDbContext dbContext = null)
        {
            if (dbContext != null)
                await dbContext.SaveChangesAsync();
            else
               await _dbContext.SaveChangesAsync();
        }


        public async Task<bool> AddressExistsAsync(int countryId, int cityId, int districtId, string street, string name)
        {
            return await _dbContext.Addresses.AnyAsync(a => a.CountryId == countryId && a.CityId == cityId
                                                                                     &&  a.DistrictId == districtId
                                                                                     && a.Street == street
                                                                                     && a.Name == name);
        }
    }
}
