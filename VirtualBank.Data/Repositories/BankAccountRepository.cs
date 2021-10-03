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


        public async Task<IEnumerable<BankAccount>> GetAll()
        {
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .Where(b => !b.Disabled)
                                                .Select(b => b.ToDomainModel())
                                                .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<BankAccount>> GetByCustomerId(int customerId)
        {
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .Where(b => b.CustomerId.Equals(customerId) && !b.Disabled)
                                                .Select(b => b.ToDomainModel())
                                                .AsNoTracking().ToListAsync();
        }

        public async Task<BankAccount> FindByIdAsync(int id)
        {
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .Where(b => b.Id.Equals(id) && !b.Disabled)
                                                .Select(b => b.ToDomainModel())
                                                .FirstOrDefaultAsync();

        }

        public async Task<BankAccount> FindByAccountNoAsync(string accountNo)
        {
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .Where(b => b.AccountNo.Equals(accountNo) && !b.Disabled)
                                                .Select(b => b.ToDomainModel())
                                                .FirstOrDefaultAsync();
        }

        public async Task<BankAccount> FindByIBANAsync(string iban)
        {
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .Where(b => b.AccountNo.Equals(iban) && !b.Disabled)
                                                .Select(b => b.ToDomainModel())
                                                .FirstOrDefaultAsync();
        }


        public async Task<BankAccount> AddAsync(BankAccount bankAccount)
        {
            await _dbContext.BankAccounts.AddAsync(bankAccount.ToEntity());

            return bankAccount;
        }


        public async Task<BankAccount> UpdateAsync(BankAccount bankAccount)
        {
            var existingBankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b => b.Id == bankAccount.Id && !b.Disabled);

            if (existingBankAccount != null)
            {
                _dbContext.Entry(existingBankAccount).State = EntityState.Detached;
            }

            _dbContext.Entry(bankAccount.ToEntity()).State = EntityState.Modified;

            return bankAccount;
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
