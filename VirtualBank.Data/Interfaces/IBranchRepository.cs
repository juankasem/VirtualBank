using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IBranchRepository
    {
        Task<IEnumerable<Branch>> GetAll();
        Task<IEnumerable<Branch>> GetByCityId(int cityId);
        Task<Branch> FindByIdAsync(int id);
        Task<Branch> FindByCodeAsync(string code);
        Task<bool> ExistsAsync(int countryId, int cityId, string branchName);

        Task<Branch> AddAsync(Branch branch);
        Task<Branch> AddAsync(VirtualBankDbContext dbContext, Branch branch);

        Task<Branch> UpdateAsync(Branch branch);
        Task<Branch> UpdateAsync(VirtualBankDbContext dbContext, Branch branch);

        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(VirtualBankDbContext dbContext, int id);


        Task SaveAsync();
        Task SaveAsync(VirtualBankDbContext dbContext);
    }
}
