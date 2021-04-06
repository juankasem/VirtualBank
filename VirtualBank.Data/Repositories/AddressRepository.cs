﻿using System;
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

        public async Task<IEnumerable<Address>> GetAll()
        {
            return await _dbContext.Addresses.Include(address => address.District)
                                             .Include(address => address.City)
                                             .Include(address => address.Country)
                                             .Where(address => address.Disabled == false)
                                             .AsNoTracking().ToListAsync();
}   

       
        public async Task<Address> FindByIdAsync(int id)
        {
            return await _dbContext.Addresses.Include(address => address.District)
                                             .Include(address => address.City)
                                             .Include(address => address.Country)
                                             .FirstOrDefaultAsync(address => address.Disabled == false);
        }


        public async Task<Address> AddAsync(Address address)
        {
            await _dbContext.Addresses.AddAsync(address);
            await SaveAsync();

            return address;
        }

        public async Task<Address> UpdateAsync(Address address)
        {
            var existingAddress = await _dbContext.Addresses
                                                  .FirstOrDefaultAsync(a => a.Id == address.Id && a.Disabled == false);

            if (existingAddress is not null)
            {
                _dbContext.Entry(existingAddress).State = EntityState.Detached;
            }

            _dbContext.Entry(address).State = EntityState.Modified;
            await SaveAsync();

            return address;
        }

        public async Task<Address> RemoveAsync(int id)
        {
            var address = await _dbContext.Addresses.FindAsync(id);

            if (address is not null)
            {
                address.Disabled = true;
                await SaveAsync();
            }

            return address;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
