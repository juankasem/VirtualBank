﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class DebitCardsRepository : IDebitCardsRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public DebitCardsRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IEnumerable<DebitCard>> GetAllAsync()
        {
            return await _dbContext.DebitCards.Include(c => c.BankAccount)
                                              .ThenInclude(c => c.Owner)
                                              .Where(c => !c.Disabled)
                                              .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<DebitCard>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbContext.DebitCards.Include(c => c.BankAccount)
                                              .ThenInclude(c => c.Owner.Id == customerId)
                                              .Where(c => !c.Disabled)
                                              .AsNoTracking().ToListAsync();
        }


        public async Task<DebitCard> FindByIdAsync(int id)
        {
            return await _dbContext.DebitCards.Include(c => c.BankAccount)
                                              .FirstOrDefaultAsync(c => c.Id == id && !c.Disabled);
        }


        public async Task<DebitCard> FindByAccountNoAsync(string accountNo)
        {
            return await _dbContext.DebitCards.Include(c => c.BankAccount)
                                              .FirstOrDefaultAsync(c => c.BankAccount.AccountNo == accountNo && !c.Disabled);
        }


        public async Task<DebitCard> FindByDebitCardNoAsync(string debitCardNo)
        {
            return await _dbContext.DebitCards.Include(c => c.BankAccount)
                                              .FirstOrDefaultAsync(c => c.DebitCardNo == debitCardNo && !c.Disabled);
                                              
        }


        public async Task<bool> ValidatePINAsync(string debitCardNo, string pin)
        {
            var isValid = false;

            var debitCard = await _dbContext.DebitCards.Include(c => c.BankAccount)
                                            .FirstOrDefaultAsync(c => c.PIN == pin && !c.Disabled);
            if (debitCard.PIN == pin)
            {
                isValid = true;
            }

            return isValid;
        }


        public async Task<DebitCard> AddAsync(DebitCard debitCard)
        {
            await _dbContext.DebitCards.AddAsync(debitCard);

            return debitCard;
        }


        public async Task<DebitCard> UpdateAsync(DebitCard debitCard)
        {
            var existingDebitCard = await _dbContext.DebitCards.FirstOrDefaultAsync(c => c.Id == debitCard.Id && !c.Disabled);

            if (existingDebitCard != null)
            {
                _dbContext.Entry(existingDebitCard).State = EntityState.Detached;
            }

            _dbContext.Entry(debitCard).State = EntityState.Modified;

            return debitCard;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var debitCard = await _dbContext.DebitCards.FindAsync(id);

            if (debitCard != null)
            {
                debitCard.Disabled = true;

                isDeleted = true;
            }

            return isDeleted;
        }
    }
}
