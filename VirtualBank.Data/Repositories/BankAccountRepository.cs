using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
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
                                                .Where(b => b.Disabled == false)
                                                .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<BankAccount>> GetByCustomerId(int customerId)
        {
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .Where(b => b.CustomerId.Equals(customerId) && b.Disabled == false)
                                                .AsNoTracking().ToListAsync();
        }

        public async Task<BankAccount> FindByIdAsync(int id)
        {
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .FirstOrDefaultAsync(b => b.Id.Equals(id) && b.Disabled == false);
        }

        public async Task<BankAccount> FindByAccountNoAsync(string accountNo)
        { 
        
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .FirstOrDefaultAsync(b => b.AccountNo.Equals(accountNo) && b.Disabled == false);
        }

        public async Task<BankAccount> FindByIBANAsync(string iban)
        {
            return await _dbContext.BankAccounts.Include(b => b.Owner)
                                                .Include(b => b.Currency)
                                                .FirstOrDefaultAsync(b => b.IBAN.Equals(iban) && b.Disabled == false);
        }


        public async Task<BankAccount> AddAsync(BankAccount bankAccount)
        {
            await _dbContext.BankAccounts.AddAsync(bankAccount);
            await SaveAsync();

            return bankAccount;
        }


        public async Task<BankAccount> AddAsync(VirtualBankDbContext dbContext, BankAccount bankAccount)
        {
            await dbContext.BankAccounts.AddAsync(bankAccount);
            await SaveAsync(dbContext);

            return bankAccount;
        }

        public async Task<BankAccount> UpdateAsync(BankAccount bankAccount)
        {
            var existingBankAccount = await _dbContext.BankAccounts
                                                      .FirstOrDefaultAsync(b => b.Id == bankAccount.Id && b.Disabled == false);

            if (existingBankAccount is not null)
            {
                _dbContext.Entry(existingBankAccount).State = EntityState.Detached;
            }

            _dbContext.Entry(bankAccount).State = EntityState.Modified;
            await SaveAsync();

            return bankAccount;
        }


        public async Task<BankAccount> UpdateAsync(VirtualBankDbContext dbContext, BankAccount bankAccount)
       
        {
            var existingBankAccount = await dbContext.BankAccounts
                                                      .FirstOrDefaultAsync(b => b.Id == bankAccount.Id && b.Disabled == false);

            if (existingBankAccount is not null)
            {
                dbContext.Entry(existingBankAccount).State = EntityState.Detached;
            }

            dbContext.Entry(bankAccount).State = EntityState.Modified;
            await SaveAsync(dbContext);

            return bankAccount;
        }


        public async Task<BankAccount> RemoveAsync(int id)
        {
            var bankAccount = await _dbContext.BankAccounts.FindAsync(id);

            if (bankAccount is not null)
            {
                bankAccount.Disabled = true;
                await SaveAsync();
            }

            return bankAccount;
        }


        public async Task<BankAccount> RemoveAsync(VirtualBankDbContext dbContext, int id)
        {
            var bankAccount = await dbContext.BankAccounts.FindAsync(id);

            if (bankAccount is not null)
            {
                bankAccount.Disabled = true;
                await SaveAsync(dbContext);
            }

            return bankAccount;
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
