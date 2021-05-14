using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface ICashTransactionsRepository
    {
        Task<IEnumerable<CashTransaction>> GetAll();
        Task<IEnumerable<CashTransaction>> GetDepositsByIBAN(string iban);
        Task<IEnumerable<CashTransaction>> GetByIBAN(string iban, int lastDays);
        Task<CashTransaction> GetLastAsync(string iban);
        Task<CashTransaction> FindByIdAsync(int id);
        Task<CashTransaction> FindByIBANAsync(string iban);
        Task<CashTransaction> AddAsync(CashTransaction transaction);
        Task<CashTransaction> AddAsync(VirtualBankDbContext dbContext, CashTransaction transaction);
        Task<CashTransaction> UpdateAsync(CashTransaction transaction);
        Task<CashTransaction> UpdateAsync(VirtualBankDbContext dbContext, CashTransaction transaction);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveAsync(VirtualBankDbContext dbContext, int id);
        Task SaveAsync();
        Task SaveAsync(VirtualBankDbContext dbContext);

    }
}
