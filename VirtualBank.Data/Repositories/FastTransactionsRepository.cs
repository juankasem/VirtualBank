﻿using System;
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
            return await _dbContext.FastTransactions.Include(f => f.Account)
                                                    .ThenInclude(f => f.Branch)
                                                    .Where(f => !f.Disabled)
                                                    .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<FastTransaction>> GetByAccountId(int accountId)
        {
            return await _dbContext.FastTransactions.Include(f => f.Account)
                                                    .ThenInclude(f => f.Branch)
                                                    .Where(f => f.AccountId == accountId && !f.Disabled)
                                                    .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<FastTransaction>> GetByIBAN(string iban)
        {
            return await _dbContext.FastTransactions.Include(f => f.Account)
                                                    .ThenInclude(f => f.Branch)
                                                    .Where(f => f.Account.IBAN == iban && !f.Disabled)
                                                    .AsNoTracking().ToListAsync();
        }

        public async Task<FastTransaction> FindByIdAsync(int id)
        {
            return await _dbContext.FastTransactions.Include(f => f.Account)
                                                    .ThenInclude(f => f.Branch)
                                                    .Where(f => f.Id == id && !f.Disabled)
                                                    .FirstOrDefaultAsync();
        }

        public async Task<FastTransaction> AddAsync(FastTransaction transaction)
        {
            await _dbContext.FastTransactions.AddAsync(transaction);

            return transaction;
        }

        public async Task<FastTransaction> UpdateAsync(FastTransaction fastTx)
        {
            var existingFastTransaction = await _dbContext.FastTransactions.FirstOrDefaultAsync(f => f.Id == fastTx.Id && !f.Disabled);

            if (existingFastTransaction != null)
            {
                _dbContext.Entry(existingFastTransaction).State = EntityState.Detached;
            }

            _dbContext.Entry(fastTx).State = EntityState.Modified;

            return fastTx;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var transaction = await _dbContext.FastTransactions.FindAsync(id);
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
