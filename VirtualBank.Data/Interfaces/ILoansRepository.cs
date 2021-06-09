using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface ILoansRepository
    {
        Task<IEnumerable<Loan>> GetAllAsync();
        Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Loan>> GetByIBANdAsync(string iban);
        Task<Loan> FindByIdAsync(int id);
        Task<Loan> AddAsync(Loan loan);
        Task<Loan> UpdateAsync(Loan loan);
        Task<bool> RemoveAsync(int id);
        Task SaveAsync();
    }
}
