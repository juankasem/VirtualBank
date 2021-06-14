using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class LoansRepository : ILoansRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public LoansRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }
      

        public async Task<IEnumerable<Loan>> GetAllAsync()
        {
            return await _dbContext.Loans.Include(l => l.Customer)
                                         .Include(l => l.BankAccount)
                                         .Where(l => l.Disabled == false)
                                         .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbContext.Loans.Include(l => l.Customer)
                                         .Include(l => l.BankAccount)
                                         .Where(l => l.CustomerId == customerId && l.Disabled == false)
                                         .AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<Loan>> GetByIBANdAsync(string iban)
        {
            return await _dbContext.Loans.Include(l => l.Customer)
                                         .Include(l => l.BankAccount)
                                         .Where(l => l.BankAccount.IBAN == iban && l.Disabled == false)
                                         .AsNoTracking().ToListAsync();
        }


        public async Task<Loan> FindByIdAsync(int id)
        {
            return await _dbContext.Loans.Include(l => l.Customer)
                                         .Include(l => l.BankAccount)
                                         .FirstOrDefaultAsync(l => l.Id == id && l.Disabled == false);
        }


        public async Task<Loan> AddAsync(Loan loan)
        {
            await _dbContext.Loans.AddAsync(loan);
            await SaveAsync();

            return loan;
        }


        public async Task<Loan> UpdateAsync(Loan loan)
        {

            var existingLoan = await _dbContext.Loans.FirstOrDefaultAsync(l => l.Id == loan.Id && loan.Disabled == false);

            if (existingLoan != null)
            {
                _dbContext.Entry(existingLoan).State = EntityState.Detached;
            }

            _dbContext.Entry(loan).State = EntityState.Modified;
            await SaveAsync();

            return loan;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var loan = await _dbContext.Loans.FindAsync(id);

            if (loan != null)
            {
                loan.Disabled = true;
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
