using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data.Interfaces
{
    public interface ICreditCardsRepository
    {
        Task<IEnumerable<CreditCard>> GetAllAsync();
        Task<IEnumerable<CreditCard>> GetByCustomerIdAsync(int customerId);
        Task<CreditCard> FindByIdAsync(int id);
        Task<CreditCard> FindByCreditCardNoAsync(string creditCardNo);
        Task<CreditCard> FindByAccountNoAsync(string accountNo);
        Task<bool> ValidatePINAsync(string creditCardNo, string pin);
        Task<CreditCard> AddAsync(CreditCard creditCard);
        Task<CreditCard> UpdateAsync(CreditCard creditCard);
        Task<bool> RemoveAsync(int id);

        Task SaveAsync();
    }
}
