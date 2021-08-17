using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm);

        Task<Customer> FindByIdAsync(int id);
        Task<Customer> FindByAccountIdAsync(int accountId);
        Task<Customer> FindByAccountNoAsync(string accountNo);
        Task<Customer> FindByIBANAsync(string iban);
        Task<Customer> FindByCreditCardIdAsync(int creditCardId);
        Task<bool> CustomerExistsAsync(Customer customer);
        Task<Customer> AddAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task<bool> RemoveAsync(int id);
    }
}
