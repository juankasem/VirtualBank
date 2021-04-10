using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IDistrictsRepository
    {

        Task<IEnumerable<District>> GetAll();
        Task<District> FindByIdAsync(int id);
        Task<District> AddAsync(District district);
        Task<District> UpdateAsync(District district);
        Task<bool> RemoveAsync(int id);

        Task SaveAsync();
    }
}
