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
                                                    .Where(c => c.Disabled == false)
                                                    .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<FastTransaction>> GetByAccountId(int accountId)
        {
            return await _dbContext.FastTransactions.Include(c => c.Account)
                                                    .Include(c => c.Branch)
                                                    .Where(c => c.AccountId == accountId && c.Disabled == false)
                                                    .AsNoTracking().ToListAsync();
        }


        public async Task<FastTransaction> FindByIdAsync(int id)
        {
            return await _dbContext.FastTransactions.Include(c => c.Account)
                                                    .Include(c => c.Branch)
                                                    .Where(c => c.Id == id && c.Disabled == false)
                                                    .FirstOrDefaultAsync();
        }


        public async Task<FastTransaction> AddAsync(FastTransaction transaction)
        {
            await _dbContext.FastTransactions.AddAsync(transaction);
            await SaveAsync();

            return transaction;
        }

        public async Task<FastTransaction> UpdateAsync(FastTransaction transaction)
        {
            var existingFastTransaction = await _dbContext.FastTransactions.Where(c => c.Id == transaction.Id && transaction.Disabled == false)
                                                                           .FirstOrDefaultAsync();

            if (existingFastTransaction is not null)
            {
                _dbContext.Entry(existingFastTransaction).State = EntityState.Detached;
            }

            _dbContext.Entry(transaction).State = EntityState.Modified;
            await SaveAsync();

            return transaction;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var transaction = await _dbContext.FastTransactions.FindAsync(id);
            var isDeleted = false;

            if (transaction is not null)
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
