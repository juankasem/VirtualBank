﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Data.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<IEnumerable<BankAccount>> GetAll();
        Task<IEnumerable<BankAccount>> GetByCustomerId(int customerId);
        Task<BankAccount> FindByIdAsync(int id);
        Task<BankAccount> FindByAccountNoAsync(string accountNo);
        Task<BankAccount> FindByIBANAsync(string iban);
        Task<BankAccount> GetLastByIBANAsync(string iban);
        Task AddAsync(BankAccount bankAccount);
        Task UpdateAsync(BankAccount bankAccount);
        Task<bool> RemoveAsync(int id);
    }
}
