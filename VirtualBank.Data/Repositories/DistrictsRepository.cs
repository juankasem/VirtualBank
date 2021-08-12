using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class DistrictsRepository : IDistrictsRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public DistrictsRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<District>> GetAllAsync()
        {
            return await _dbContext.Districts.Include(d => d.City)
                                             .Where(district => !district.Disabled)
                                             .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<District>> GetByCityIdAsync(int cityId)
        {
            return await _dbContext.Districts.Include(c => c.City)
                                             .Where(district => district.CityId == cityId && !district.Disabled)
                                             .AsNoTracking().ToListAsync();
        }


        public async Task<District> FindByIdAsync(int id)
        {
            return await _dbContext.Districts.Include(d => d.City)
                                             .FirstOrDefaultAsync(district => district.Id == id && !district.Disabled);
        }


        public async Task<District> AddAsync(District district)
        {
            await _dbContext.Districts.AddAsync(district);
            await SaveAsync();

            return district;
        }

        public async Task<District> UpdateAsync(District district)
        {
            var existingDistrict = await _dbContext.Districts.FirstOrDefaultAsync(d => d.Id == district.Id && !district.Disabled);

            if (existingDistrict != null)
            {
                _dbContext.Entry(existingDistrict).State = EntityState.Detached;
            }

            _dbContext.Entry(district).State = EntityState.Modified;
            await SaveAsync();

            return district;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var district = await _dbContext.Districts.FindAsync(id);

            if (district != null)
            {
                district.Disabled = true;
                await SaveAsync();

                isDeleted = true;
            }

            return isDeleted;
        }


        public async Task<bool> DistrictExists(int districtId)
        {
            return await _dbContext.Districts.AnyAsync(d => d.Id == districtId);
        }


        public async Task<bool> DistrictNameExists(int cityId, string districtName)
        {
            return await _dbContext.Districts.AnyAsync(d => d.CityId == cityId && d.Name == districtName);
        }


        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        } 
    }
}
