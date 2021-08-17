using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address> FindByIdAsync(int id);
        Task<Address> AddAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task<bool> RemoveAsync(int id);
        Task<bool> AddressExistsAsync(int countryId, int cityId, int districtId, string street, string name);
    }
}
