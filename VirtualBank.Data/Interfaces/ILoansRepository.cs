using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Data.Interfaces
{
    public interface ILoansRepository
    {
        Task<IEnumerable<Loan>> GetAllAsync();
        Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Loan>> GetByIBANdAsync(string iban);
        Task<Loan> FindByIdAsync(Guid id);
        Task AddAsync(Loan loan);
        Task UpdateAsync(Loan loan);
        Task<bool> RemoveAsync(int id);
    }
}
