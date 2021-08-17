using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface IDebitCardsRepository
    {
        Task<IEnumerable<DebitCard>> GetAllAsync();
        Task<IEnumerable<DebitCard>> GetByCustomerIdAsync(int customerId);
        Task<DebitCard> FindByIdAsync(int id);
        Task<DebitCard> FindByAccountNoAsync(string accountNo);
        Task<DebitCard> FindByDebitCardNoAsync(string debitCardNo);
        Task<bool> ValidatePINAsync(string debitCardNo, string pin);
        Task<DebitCard> AddAsync(DebitCard debitCard);
        Task<DebitCard> UpdateAsync(DebitCard debitCard);
        Task<bool> RemoveAsync(int id);
    }
}
