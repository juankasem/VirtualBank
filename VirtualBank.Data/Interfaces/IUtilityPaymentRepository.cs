using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Data.Interfaces
{
    public interface IUtilityPaymentRepository
    {
        Task<IEnumerable<UtilityPayment>> ListAllAsync();
        Task<IEnumerable<UtilityPayment>> GetByCustomerIdAsync(int customerId);
        Task<UtilityPayment> FindByIdAsync(Guid id);
        Task AddAsync(UtilityPayment utilityPayment);
        Task UpdateAsync(UtilityPayment utilityPayment);
        Task<bool> RemoveAsync(int id);
    }
}
