using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly VirtualBankDbContext _dbContext;


        public BankAccountRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IEnumerable<BankAccount>> GetAll() =>
               await _dbContext.BankAccounts.Include(b => b.Owner)
                                            .Include(b => b.Currency)
                                            .Where(b => !b.Disabled)
                                            .Select(b => b.ToDomainModel())
                                            .AsNoTracking().ToListAsync();



        public async Task<IEnumerable<BankAccount>> GetByCustomerId(int customerId) =>
               await _dbContext.BankAccounts.Include(b => b.Owner)
                                            .Include(b => b.Currency)
                                            .Where(b => b.CustomerId.Equals(customerId) && !b.Disabled)
                                            .Select(b => b.ToDomainModel())
                                            .AsNoTracking().ToListAsync();


        public async Task<BankAccount> FindByIdAsync(int id) =>
               await _dbContext.BankAccounts.Include(b => b.Owner)
                                            .Include(b => b.Currency)
                                            .Where(b => b.Id.Equals(id) && !b.Disabled)
                                            .Select(b => b.ToDomainModel())
                                            .FirstOrDefaultAsync();



        public async Task<BankAccount> FindByAccountNoAsync(string accountNo) =>
               await _dbContext.BankAccounts.Include(b => b.Owner)
                                            .Include(b => b.Currency)
                                            .Where(b => b.AccountNo.Equals(accountNo) && !b.Disabled)
                                            .Select(b => b.ToDomainModel())
                                            .FirstOrDefaultAsync();


        public async Task<BankAccount> FindByIBANAsync(string iban) =>
               await _dbContext.BankAccounts.Include(b => b.Owner)
                                            .Include(b => b.Currency)
                                            .Where(b => b.AccountNo.Equals(iban) && !b.Disabled)
                                            .Select(b => b.ToDomainModel())
                                            .FirstOrDefaultAsync();


        public async Task<BankAccount> GetLastByIBANAsync(string iban) =>
               await _dbContext.BankAccounts.Include(b => b.Owner)
                                            .Include(b => b.Currency)
                                            .Where(b => b.AccountNo.Equals(iban))
                                            .OrderByDescending(b => b.LastModifiedOn)
                                            .Select(b => b.ToDomainModel())
                                            .FirstOrDefaultAsync();


        public async Task<Core.Entities.BankAccount> AddAsync(BankAccount bankAccount)
        {
            var entity = bankAccount.ToEntity();

            await _dbContext.BankAccounts.AddAsync(entity);

            return entity;
        }



        public async Task<Core.Entities.BankAccount> UpdateAsync(BankAccount bankAccount)
        {
            var existingBankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(c => c.Id == bankAccount.Id);

            if (existingBankAccount != null)
            {
                _dbContext.Entry(existingBankAccount).State = EntityState.Detached;
            }

            var entity = bankAccount.ToEntity();

            _dbContext.Entry(entity).State = EntityState.Modified;

            return entity;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var bankAccount = await _dbContext.BankAccounts.FindAsync(id);

            if (bankAccount != null)
            {
                bankAccount.Disabled = true;

                isDeleted = true;
            }

            return isDeleted;
        }
    }
}
