using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public CustomerRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbContext.Customers.Include(customer => customer.Address)
                                             .Where(customer => customer.Disabled == false)
                                             .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm)
        {
            return await _dbContext.Customers.Include(customer => customer.Address)
                                             .Where(customer => (customer.FirstName.Contains(searchTerm) || customer.LastName.Contains(searchTerm)) && customer.Disabled == false)
                                             .AsNoTracking().ToListAsync();
        }


        public async Task<Customer> FindByAccountIdAsync(int accountId)
        {
            return await _dbContext.Customers.Include(customer => customer.Address)
                                             .Where(customer => customer.BankAccounts.FirstOrDefault().Id == accountId && customer.Disabled == false)
                                             .FirstOrDefaultAsync();
        }

        public async Task<Customer> FindByAccountNoAsync(string accountNo)
        {

            return await _dbContext.Customers.Include(customer => customer.Address)
                                             .Where(customer => customer.BankAccounts.FirstOrDefault().AccountNo == accountNo && customer.Disabled == false)
                                             .FirstOrDefaultAsync();
        }

        public async Task<Customer> FindByIBANAsync(string iban)
        {
            return await _dbContext.Customers.Include(customer => customer.Address)
                                             .Where(customer => customer.BankAccounts.FirstOrDefault().IBAN == iban && customer.Disabled == false)
                                             .FirstOrDefaultAsync();
        }

        public async Task<Customer> FindByIdAsync(int id)
        {
            return await _dbContext.Customers.Include(customer => customer.Address)
                                             .Where(customer => customer.Id == id && customer.Disabled == false)
                                             .FirstOrDefaultAsync();
        }

        public async Task<Customer> FindByCreditCardIdAsync(int creditCardId)
        {
            return await _dbContext.Customers.Include(customer => customer.Address)
                                             .Where(customer => customer.CreditCards.FirstOrDefault().Id == creditCardId && customer.Disabled == false)
                                             .FirstOrDefaultAsync();
        }

        public async Task<bool> CustomerExistsAsync(Customer customer)
        {
            return await _dbContext.Customers.AnyAsync(c => c.FirstName == customer.FirstName && c.LastName == customer.LastName
                                                                                              && c.FatherName == customer.FatherName);
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await SaveAsync();

            return customer;
        }

        public async Task<Customer> AddAsync(VirtualBankDbContext dbContext, Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await SaveAsync(dbContext);

            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            var existingCustomer = await _dbContext.Customers.Where(c => c.Id == customer.Id && c.Disabled == false)
                                                   .FirstOrDefaultAsync();

            if (existingCustomer != null)
            {
                _dbContext.Entry(existingCustomer).State = EntityState.Detached;
            }

            _dbContext.Entry(customer).State = EntityState.Modified;
            await SaveAsync();

            return customer;
        }

        public async Task<Customer> UpdateAsync(VirtualBankDbContext dbContext, Customer customer)
        {
            var existingCustomer = await _dbContext.Customers.Where(c => c.Id == customer.Id && c.Disabled == false)
                                                             .FirstOrDefaultAsync();

            if (existingCustomer != null)
            {
                _dbContext.Entry(existingCustomer).State = EntityState.Detached;
            }

            _dbContext.Entry(customer).State = EntityState.Modified;
            await SaveAsync(dbContext);

            return customer;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer != null)
            {
                customer.Disabled = true;
                await SaveAsync();

                isDeleted = true;
            }

            return isDeleted;
        }

        public async Task<bool> RemoveAsync(VirtualBankDbContext dbContext, int id)
        {
            var isDeleted = false;
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer != null)
            {
                customer.Disabled = true;
                await SaveAsync(dbContext);

                isDeleted = true;
            }

            return isDeleted;
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
