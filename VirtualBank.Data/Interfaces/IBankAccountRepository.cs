using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<IEnumerable<BankAccount>> GetAll();
        Task<IEnumerable<BankAccount>> GetByCustomerId(int customerId);
        Task<BankAccount> FindByIdAsync(int id);
        Task<BankAccount> FindByAccountNoAsync(string accountNo);
        Task<BankAccount> FindByIBANAsync(string iban);
        Task<BankAccount> AddAsync(BankAccount bankAccount);
        Task<BankAccount> AddAsync(VirtualBankDbContext dbContext, BankAccount bankAccount);
        Task<BankAccount> UpdateAsync(BankAccount bankAccount);
        Task<BankAccount> UpdateAsync(VirtualBankDbContext dbContext, BankAccount bankAccount);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(VirtualBankDbContext dbContext, int id);
        Task SaveAsync();
        Task SaveAsync(VirtualBankDbContext dbContext);
    }
}
