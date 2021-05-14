using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class CreditCardsRepository : ICreditCardsRepository
    {

        private readonly VirtualBankDbContext _dbContext;

        public CreditCardsRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CreditCard>> GetAllAsync()
        {
            return await _dbContext.CreditCards.Include(c => c.BankAccount)
                                               .ThenInclude(c => c.Owner)
                                               .Where(c => c.Disabled == false)
                                               .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<CreditCard>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbContext.CreditCards.Include(c => c.BankAccount)
                                            .ThenInclude(c => c.Owner.Id == customerId)
                                            .Where(c => c.Disabled == false)
                                            .AsNoTracking().ToListAsync();
        }


        public async Task<CreditCard> FindByIdAsync(int id)
        {
            return await _dbContext.CreditCards.Include(c => c.BankAccount)
                                               .Where(c => c.Id == id && c.Disabled == false)
                                               .FirstOrDefaultAsync();
        }


        public async Task<CreditCard> FindByAccountNoAsync(string accountNo)
        {
            return await _dbContext.CreditCards.Include(c => c.BankAccount)
                                                          .Where(c => c.BankAccount.AccountNo == accountNo && c.Disabled == false)
                                                          .FirstOrDefaultAsync();
        }


        public async Task<CreditCard> AddAsync(CreditCard creditCard)
        {
            await _dbContext.CreditCards.AddAsync(creditCard);
            await SaveAsync();

            return creditCard;
        }

        public async Task<CreditCard> UpdateAsync(CreditCard creditCard)
        {
            var existingCreditCard = await _dbContext.CreditCards
                                                     .Where(c => c.Id == creditCard.Id && c.Disabled == false)
                                                     .FirstOrDefaultAsync();

            if (existingCreditCard != null)
            {
                _dbContext.Entry(existingCreditCard).State = EntityState.Detached;
            }

            _dbContext.Entry(creditCard).State = EntityState.Modified;
            await SaveAsync();

            return creditCard;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var creditCard = await _dbContext.CreditCards.FindAsync(id);

            if (creditCard != null)
            {
                creditCard.Disabled = true;
                await SaveAsync();

                isDeleted = true;
            }

            return isDeleted;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
