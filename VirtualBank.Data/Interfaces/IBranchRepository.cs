using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IBranchRepository
    {
        Task<IEnumerable<Branch>> ListAsync(int countryId, int cityId, int districtId);      
        Task<IEnumerable<Branch>> SearchByNameAsync(string searchTerm);
        Task<Branch> FindByIdAsync(int id);
        Task<Branch> FindByCodeAsync(string code);
        Task<bool> ExistsAsync(int countryId, int cityId, string branchName);
        Task<Branch> AddAsync(Branch branch);
        Task<Branch> UpdateAsync(Branch branch);
        Task<bool> RemoveAsync(int id);
    }
}
