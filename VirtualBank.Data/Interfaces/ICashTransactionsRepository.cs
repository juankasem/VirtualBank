using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Data.Interfaces
{
    public interface ICashTransactionsRepository
    {
        Task<IEnumerable<CashTransaction>> GetAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<CashTransaction>> GetDepositsByIBANAsync(string iban);
        Task<IEnumerable<CashTransaction>> GetByIBANAsync(string iban, int lastDays, int pageNumber, int pageSize);
        Task<IEnumerable<CashTransaction>> GetLatestByIBANAsync(string iban);
        Task<CashTransaction> GetLastByIBANAsync(string iban);
        Task<CashTransaction> FindByIdAsync(Guid id);
        Task<CashTransaction> FindByIBANAsync(string iban);
        Task AddAsync(CashTransaction transaction);
        Task UpdateAsync(CashTransaction transaction);
    }
}
