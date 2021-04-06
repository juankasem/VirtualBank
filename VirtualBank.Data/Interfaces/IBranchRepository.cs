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
        Task<Branch> AddAsync(Branch branch);
        Task<Branch> UpdateAsync(Branch branch);
        Task<Branch> RemoveAsync(int id);
        Task SaveAsync();
    }
}
