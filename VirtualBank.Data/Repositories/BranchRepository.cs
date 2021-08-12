using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class BranchRepository :  IBranchRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public BranchRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Branch>> ListAsync(int countryId, int cityId, int districtId)
        {
            IEnumerable<Branch> branches;

            branches = await _dbContext.Branches.Include(branch => branch.Address)
                                                .Where(branch => !branch.Disabled)
                                                .AsNoTracking().ToListAsync();

            if (countryId > 0)
                branches = branches.Where(b => b.Address.CountryId == countryId).ToList();

            if (cityId > 0)
                branches = branches.Where(b => b.Address.CityId == cityId).ToList();

            if (districtId > 0)
                branches = branches.Where(b => b.Address.DistrictId == districtId).ToList();

            return branches;
        }

        public async Task<IEnumerable<Branch>> SearchByNameAsync(string searchTerm)
        {
            return await _dbContext.Branches.Include(branch => branch.Address)
                                            .Where(branch => branch.Name.Contains(searchTerm) && !branch.Disabled)
                                            .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<Branch>> GetByCityIdAsync(int cityId)
        {
            return await _dbContext.Branches.Include(branch => branch.Address)
                                            .Where(branch => branch.Address.CityId == cityId && !branch.Disabled)
                                            .AsNoTracking().ToListAsync();
        }


        public async Task<Branch> FindByIdAsync(int id)
        {
            return await _dbContext.Branches.Include(branch => branch.Address)
                                            .FirstOrDefaultAsync(branch => branch.Id == id && !branch.Disabled);
        }


        public async Task<Branch> FindByCodeAsync(string code)
        {
            return await _dbContext.Branches.Include(branch => branch.Address)
                                            .FirstOrDefaultAsync(branch => branch.Code == code && !branch.Disabled);
        }

        public async Task<bool> ExistsAsync(int countryId, int cityId, string branchName)
        {
            return await _dbContext.Branches.AnyAsync(b => b.Address.CountryId == countryId && b.Address.CityId == cityId &&
                                                           b.Name.Equals(branchName));
        }

        public async Task<Branch> AddAsync(Branch branch)
        {
            await _dbContext.Branches.AddAsync(branch);
            await SaveAsync();

            return branch;
        }


        public async Task<Branch> AddAsync(Branch branch, VirtualBankDbContext dbContext)
        {
            await dbContext.Branches.AddAsync(branch);
            await SaveAsync(dbContext);

            return branch;
        }


        public async Task<Branch> UpdateAsync(Branch branch)
        {
            var existingBranch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == branch.Id && !b.Disabled);

            if (existingBranch != null)
            {
                _dbContext.Entry(existingBranch).State = EntityState.Detached;
            }

            _dbContext.Entry(branch).State = EntityState.Modified;
            await SaveAsync();

            return branch;
        }


        public async Task<Branch> UpdateAsync(Branch branch, VirtualBankDbContext dbContext)
        {
            var existingBranch = await dbContext.Branches.FirstOrDefaultAsync(b => b.Id == branch.Id && !b.Disabled);

            if (existingBranch != null)
            {
                dbContext.Entry(existingBranch).State = EntityState.Detached;
            }

            dbContext.Entry(branch).State = EntityState.Modified;
            await SaveAsync(dbContext);

            return branch;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var branch = await _dbContext.Branches.FindAsync(id);

            if (branch != null)
            {
                branch.Disabled = true;
                await SaveAsync();

                isDeleted = true;
            }

            return isDeleted;
        }


        public async Task<bool> RemoveAsync(int id, VirtualBankDbContext dbContext)
        {
            var isDeleted = false;
            var branch = await dbContext.Branches.FindAsync(id);

            if (branch != null)
            {
                branch.Disabled = true;
                await SaveAsync(dbContext);

                isDeleted = true;
            }

            return isDeleted;
        }


        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }


        public async Task SaveAsync(VirtualBankDbContext dbContext)
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
