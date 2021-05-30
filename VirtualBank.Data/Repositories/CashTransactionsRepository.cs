using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class CashTransactionsRepository : ICashTransactionsRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public CashTransactionsRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CashTransaction>> GetAllAsync()
        {
            return await _dbContext.CashTransactions.Where(c => c.Disabled == false)
                                                    .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<CashTransaction>> GetByIBANAsync(string iban, int lastDays)
        {
            return await _dbContext.CashTransactions.Where(c => (c.From == iban || c.To == iban)
                                                                && DateTime.UtcNow.Subtract(c.TransactionDate).TotalDays <= lastDays
                                                                && c.Disabled == false)
                                                                .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<CashTransaction>> GetLastByIBANAsync(string iban)
        {
            return await _dbContext.CashTransactions.Where(c => c.From == iban && c.Disabled == false)
                                                    .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<CashTransaction>> GetDepositsByIBANAsync(string iban)
        {
            return await _dbContext.CashTransactions.Where(c => c.To == iban && c.Disabled == false)
                                                    .AsNoTracking().ToListAsync();
        }


        public async Task<CashTransaction> GetLastAsync(string iban)
        {
           return await _dbContext.CashTransactions.Where(c => (c.From == iban || c.To == iban)
                                                          && c.Disabled == false)
                                                   .OrderByDescending(c => c.CreatedAt)
                                                   .FirstOrDefaultAsync();
        }



        public async Task<CashTransaction> FindByIdAsync(int id)
        {
            return await _dbContext.CashTransactions.Where(c => c.Id == id && c.Disabled == false)
                                                    .FirstOrDefaultAsync();
        }

        public async Task<CashTransaction> FindByIBANAsync(string iban)
        {
            return await _dbContext.CashTransactions.Where(c => (c.From == iban || c.To == iban)
                                                            && c.Disabled == false)
                                                    .OrderByDescending(c => c.TransactionDate)
                                                    .FirstOrDefaultAsync();
        }


        public async Task<CashTransaction> AddAsync(CashTransaction transaction)
        {
            await _dbContext.CashTransactions.AddAsync(transaction);
            await SaveAsync();

            return transaction;
        }


        public async Task<CashTransaction> AddAsync(VirtualBankDbContext dbContext, CashTransaction transaction)
        {
            await dbContext.CashTransactions.AddAsync(transaction);
            await SaveAsync(dbContext);

            return transaction;
        }


        public async Task<CashTransaction> UpdateAsync(CashTransaction transaction)
        {
            var existingCashTransaction = await _dbContext.CashTransactions.Where(c => c.Id == transaction.Id && transaction.Disabled == false)
                                                                           .FirstOrDefaultAsync();

            if (existingCashTransaction != null)
            {
                _dbContext.Entry(existingCashTransaction).State = EntityState.Detached;
            }

            _dbContext.Entry(transaction).State = EntityState.Modified;
            await SaveAsync();

            return transaction;
        }

        public async Task<CashTransaction> UpdateAsync(VirtualBankDbContext dbContext, CashTransaction transaction)
        {
            var existingCashTransaction = await dbContext.CashTransactions.Where(c => c.Id == transaction.Id && transaction.Disabled == false)
                                                                          .FirstOrDefaultAsync();

            if (existingCashTransaction != null)
            {
                dbContext.Entry(existingCashTransaction).State = EntityState.Detached;
            }

            dbContext.Entry(transaction).State = EntityState.Modified;
            await SaveAsync(dbContext);

            return transaction;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var transaction = await _dbContext.CashTransactions.FindAsync(id);
            var isDeleted = false;

            if (transaction != null)
            {
                transaction.Disabled = true;
                await SaveAsync();

                isDeleted = true;
            }

            return isDeleted;
        }

        public async Task<bool> RemoveAsync(VirtualBankDbContext dbContext, int id)
        {
            var transaction = await dbContext.CashTransactions.FindAsync(id);
            var isDeleted = false;

            if (transaction != null)
            {
                transaction.Disabled = true;
                await SaveAsync(dbContext);

                isDeleted = true;

            }

            return isDeleted; 
        }


        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }


        public async Task SaveAsync(VirtualBankDbContext dbContext)
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
