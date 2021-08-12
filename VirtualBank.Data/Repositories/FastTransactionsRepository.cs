using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class FastTransactionsRepository : IFastTransactionsRepository
    {

        private readonly VirtualBankDbContext _dbContext;

        public FastTransactionsRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FastTransaction>> GetAll()
        {
            return await _dbContext.FastTransactions.Include(c => c.Account)
                                                    .Include(c => c.Branch)
                                                    .Where(c => !c.Disabled)
                                                    .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<FastTransaction>> GetByAccountId(int accountId)
        {
            return await _dbContext.FastTransactions.Include(c => c.Account)
                                                    .Include(c => c.Branch)
                                                    .Where(c => c.AccountId == accountId && !c.Disabled)
                                                    .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<FastTransaction>> GetByIBAN(string iban)
        {
            return await _dbContext.FastTransactions.Include(c => c.Account)
                                                             .Include(c => c.Branch)
                                                             .Where(c => c.Account.IBAN == iban && !c.Disabled)
                                                             .AsNoTracking().ToListAsync();
        }


        public async Task<FastTransaction> FindByIdAsync(int id)
        {
            return await _dbContext.FastTransactions.Include(c => c.Account)
                                                    .Include(c => c.Branch)
                                                    .Where(c => c.Id == id && !c.Disabled)
                                                    .FirstOrDefaultAsync();
        }


        public async Task<FastTransaction> AddAsync(FastTransaction transaction)
        {
            await _dbContext.FastTransactions.AddAsync(transaction);
            await SaveAsync();

            return transaction;
        }

        public async Task<FastTransaction> UpdateAsync(FastTransaction fastTx)
        {
            var existingFastTransaction = await _dbContext.FastTransactions.FirstOrDefaultAsync(c => c.Id == fastTx.Id && !c.Disabled);

            if (existingFastTransaction != null)
            {
                _dbContext.Entry(existingFastTransaction).State = EntityState.Detached;
            }

            _dbContext.Entry(fastTx).State = EntityState.Modified;
            await SaveAsync();

            return fastTx;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var transaction = await _dbContext.FastTransactions.FindAsync(id);
            var isDeleted = false;

            if (transaction != null)
            {
                transaction.Disabled = true;
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
