using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Data.Interfaces
{
    public interface IUtilityPaymentRepository
    {
        Task<IEnumerable<UtilityPayment>> ListAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<UtilityPayment>> GetByIBANAsync(string iban, int pageNumber, int pageSize);
        Task<UtilityPayment> FindByIdAsync(Guid id);
        Task AddAsync(UtilityPayment utilityPayment);
        Task UpdateAsync(UtilityPayment utilityPayment);
        Task<bool> RemoveAsync(int id);
    }
}
