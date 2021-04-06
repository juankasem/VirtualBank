using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAll();
        Task<Address> FindByIdAsync(int id);
        Task<Address> AddAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task<Address> RemoveAsync(int id);
        Task SaveAsync();
    }
}
