using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Domain.Models;
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
            return await _dbContext.CashTransactions.Where(c => !c.Disabled)
                                                    .Select(c => c.ToDomainModel())
                                                    .AsNoTracking()
                                                    .ToListAsync();
        }


        public async Task<IEnumerable<CashTransaction>> GetByIBANAsync(string iban, int lastDays)
        {
            return await _dbContext.CashTransactions.Where(c => (c.From == iban || c.To == iban)
                                                                && DateTime.UtcNow.Subtract(c.TransactionDate).TotalDays <= lastDays
                                                                && !c.Disabled)
                                                    .Select(c => c.ToDomainModel())
                                                    .AsNoTracking()
                                                    .ToListAsync();
        }


        public async Task<IEnumerable<CashTransaction>> GetLatestByIBANAsync(string iban) =>
               await _dbContext.CashTransactions.Where(c => c.From == iban && !c.Disabled)
                                                .Select(c => c.ToDomainModel())
                                                .AsNoTracking()
                                                .ToListAsync();



        public async Task<IEnumerable<CashTransaction>> GetDepositsByIBANAsync(string iban) =>
               await _dbContext.CashTransactions.Where(c => c.To == iban && !c.Disabled)
                                                .Select(c => c.ToDomainModel())
                                                .AsNoTracking()
                                                .ToListAsync();


        public async Task<CashTransaction> GetLastByIBANAsync(string iban) =>
               await _dbContext.CashTransactions.Where(c => (c.From == iban || c.To == iban) && !c.Disabled)
                                                .Select(c => c.ToDomainModel())
                                                .OrderByDescending(c => c.ModificationInfo.LastModifiedOn)
                                                .FirstOrDefaultAsync();



        public async Task<CashTransaction> FindByIdAsync(int id) =>
               await _dbContext.CashTransactions.Where(c => c.Id == id && !c.Disabled)
                                                .Select(c => c.ToDomainModel())
                                                .FirstOrDefaultAsync();



        public async Task<CashTransaction> FindByIBANAsync(string iban) =>
               await _dbContext.CashTransactions.Where(c => (c.From == iban || c.To == iban) && !c.Disabled)
                                                .Select(c => c.ToDomainModel())
                                                .FirstOrDefaultAsync();



        public async Task AddAsync(CashTransaction transaction) =>
            await _dbContext.CashTransactions.AddAsync(transaction.ToEntity(transaction.SenderRemainingBalance.Amount.Value,
                                                                            transaction.RecipientRemainingBalance.Amount.Value));


        public async Task UpdateAsync(CashTransaction transaction)
        {
            var existingCashTransaction = await _dbContext.CashTransactions.FirstOrDefaultAsync(c => c.Id == transaction.Id && !c.Disabled);

            if (existingCashTransaction != null)
            {
                _dbContext.Entry(existingCashTransaction).State = EntityState.Detached;
            }

            var entity = transaction.ToEntity(transaction.SenderRemainingBalance.Amount.Value,
                                              transaction.RecipientRemainingBalance.Amount.Value);

            _dbContext.Entry(entity).State = EntityState.Modified;

        }


        public async Task<bool> RemoveAsync(int id)
        {
            var transaction = await _dbContext.CashTransactions.FindAsync(id);
            var isDeleted = false;

            if (transaction != null)
            {
                transaction.Disabled = true;

                isDeleted = true;
            }

            return isDeleted;
        }
    }
}
