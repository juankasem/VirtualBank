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
        Task<BankAccount> AddAsync(BankAccount bankAccount, VirtualBankDbContext dbContext);
        Task<BankAccount> UpdateAsync(BankAccount bankAccount);
        Task<BankAccount> UpdateAsync(BankAccount bankAccount, VirtualBankDbContext dbContext);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(int id, VirtualBankDbContext dbContext);
        Task SaveAsync();
        Task SaveAsync(VirtualBankDbContext dbContext);
    }
}
