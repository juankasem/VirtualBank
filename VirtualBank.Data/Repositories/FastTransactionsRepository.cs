using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Domain.Models;
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


        public async Task<IEnumerable<FastTransaction>> GetAll() =>
                   await _dbContext.FastTransactions.Include(f => f.RecipientBankAccount)
                                                    .ThenInclude(f => f.Branch)
                                                    .Where(f => !f.Disabled)
                                                    .Select(f => f.ToDomainModel())
                                                    .AsNoTracking().ToListAsync();


        public async Task<IEnumerable<FastTransaction>> GetByAccountId(int accountId) =>
                   await _dbContext.FastTransactions.Include(f => f.RecipientBankAccount)
                                                    .ThenInclude(f => f.Branch)
                                                    .Where(f => f.RecipientBankAccountId == accountId && !f.Disabled)
                                                    .Select(f => f.ToDomainModel())
                                                    .AsNoTracking().ToListAsync();


        public async Task<IEnumerable<FastTransaction>> GetByIBAN(string iban) =>
                   await _dbContext.FastTransactions.Include(f => f.RecipientBankAccount)
                                                    .ThenInclude(f => f.Branch)
                                                    .Where(f => f.RecipientBankAccount.IBAN == iban && !f.Disabled)
                                                    .Select(f => f.ToDomainModel())
                                                    .AsNoTracking().ToListAsync();


        public async Task<FastTransaction> FindByIdAsync(int id) =>
                 await _dbContext.FastTransactions.Include(f => f.RecipientBankAccount)
                                                    .ThenInclude(f => f.Branch)
                                                    .Where(f => f.Id == id && !f.Disabled)
                                                    .Select(f => f.ToDomainModel())
                                                    .FirstOrDefaultAsync();


        public async Task AddAsync(FastTransaction transaction)
        {
            var entity = transaction.ToEntity();
            await _dbContext.FastTransactions.AddAsync(entity);
        }

        public async Task UpdateAsync(FastTransaction fastTransaction)
        {
            var entity = fastTransaction.ToEntity();
            var existingFastTransaction = await _dbContext.FastTransactions.FirstOrDefaultAsync(f => f.Id == fastTransaction.Id && !f.Disabled);

            if (existingFastTransaction != null)
            {
                _dbContext.Entry(existingFastTransaction).State = EntityState.Detached;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
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
