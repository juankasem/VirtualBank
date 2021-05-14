using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IFastTransactionsRepository
    {
        Task<IEnumerable<FastTransaction>> GetAll();
        Task<IEnumerable<FastTransaction>> GetByAccountId(int accountId);
        Task<IEnumerable<FastTransaction>> GetByIBAN(string iban);
        Task<FastTransaction> FindByIdAsync(int id);
        Task<FastTransaction> AddAsync(FastTransaction transaction);
        Task<FastTransaction> UpdateAsync(FastTransaction transaction);
        Task<bool> RemoveAsync(int id);
        Task SaveAsync();
    }
}
