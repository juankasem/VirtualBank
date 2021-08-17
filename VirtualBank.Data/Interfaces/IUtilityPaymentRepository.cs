using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IUtilityPaymentRepository
    {
        Task<IEnumerable<UtilityPayment>> ListAllAsync();
        Task<IEnumerable<UtilityPayment>> GetByCustomerIdAsync(int customerId);
        Task<UtilityPayment> FindByIdAsync(int id);

        Task<UtilityPayment> AddAsync(UtilityPayment utilityPayment);
        Task<UtilityPayment> UpdateAsync(UtilityPayment utilityPayment);
        Task<bool> RemoveAsync(int id);
    }
}
