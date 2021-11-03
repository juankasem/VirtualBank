using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Domain.Models;
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


        public async Task<IEnumerable<Loan>> GetAllAsync() =>
             await _dbContext.Loans.Include(l => l.Customer)
                                   .Include(l => l.BankAccount)
                                   .Where(l => !l.Disabled)
                                   .Select(loan => loan.ToDomainModel())
                                   .AsNoTracking().ToListAsync();



        public async Task<IEnumerable<Loan>> GetByCustomerIdAsync(int customerId) =>
                  await _dbContext.Loans.Include(l => l.Customer)
                                        .Include(l => l.BankAccount)
                                        .Where(l => l.CustomerId == customerId && !l.Disabled)
                                        .Select(loan => loan.ToDomainModel())
                                        .AsNoTracking().ToListAsync();



        public async Task<IEnumerable<Loan>> GetByIBANdAsync(string iban) =>
                  await _dbContext.Loans.Include(l => l.Customer)
                                         .Include(l => l.BankAccount)
                                         .Where(l => l.BankAccount.IBAN == iban && !l.Disabled)
                                         .Select(loan => loan.ToDomainModel())
                                         .AsNoTracking().ToListAsync();


        public async Task<Loan> FindByIdAsync(Guid id) =>
                  await _dbContext.Loans.Include(l => l.Customer)
                                        .Include(l => l.BankAccount)
                                        .Where(loan => loan.Id == id && !loan.Disabled)
                                        .Select(loan => loan.ToDomainModel())
                                        .FirstOrDefaultAsync();


        public async Task AddAsync(Loan loan)
        {
            var entity = loan.ToEntity();
            await _dbContext.Loans.AddAsync(entity);
        }


        public async Task UpdateAsync(Loan loan)
        {
            var entity = loan.ToEntity();

            var existingLoan = await _dbContext.Loans.FirstOrDefaultAsync(l => l.Id == loan.Id && !l.Disabled);

            if (existingLoan != null)
            {
                _dbContext.Entry(existingLoan).State = EntityState.Detached;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
        }


        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var loan = await _dbContext.Loans.FindAsync(id);

            if (loan != null)
            {
                loan.Disabled = true;

                isDeleted = true;
            }

            return isDeleted;
        }
    }
}
