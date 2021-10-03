using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Data.Interfaces
{
    public interface ICashTransactionsRepository
    {
        Task<IEnumerable<CashTransaction>> GetAllAsync();
        Task<IEnumerable<CashTransaction>> GetDepositsByIBANAsync(string iban);
        Task<IEnumerable<CashTransaction>> GetByIBANAsync(string iban, int lastDays);
        Task<IEnumerable<CashTransaction>> GetLastByIBANAsync(string iban);
        Task<CashTransaction> GetLastAsync(string iban);
        Task<CashTransaction> FindByIdAsync(int id);
        Task<CashTransaction> FindByIBANAsync(string iban);
        Task<CashTransaction> AddAsync(CashTransaction transaction);
        Task<CashTransaction> UpdateAsync(CashTransaction transaction);
        Task<bool> RemoveAsync(int id);
    }
}
