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
            return await _dbContext.Customers.Include(c => c.Address)
                                             .Where(c => !c.Disabled)
                                             .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm)
        {
            return await _dbContext.Customers.Include(c => c.Address)
                                             .Where(c => (c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm)) && !c.Disabled)
                                             .AsNoTracking().ToListAsync();
        }


        public async Task<Customer> FindByAccountIdAsync(int accountId)
        {
            return await _dbContext.Customers.Include(c => c.Address)
                                             .Where(c => c.BankAccounts.FirstOrDefault().Id == accountId && !c.Disabled)
                                             .FirstOrDefaultAsync();
        }

        public async Task<Customer> FindByAccountNoAsync(string accountNo)
        {

            return await _dbContext.Customers.Include(c => c.Address)
                                             .Where(c => c.BankAccounts.FirstOrDefault().AccountNo == accountNo && !c.Disabled)
                                             .FirstOrDefaultAsync();
        }

        public async Task<Customer> FindByIBANAsync(string iban)
        {
            return await _dbContext.Customers.Include(c => c.Address)
                                             .Where(c => c.BankAccounts.FirstOrDefault().IBAN == iban && !c.Disabled)
                                             .FirstOrDefaultAsync();
        }

        public async Task<Customer> FindByIdAsync(int id)
        {
            return await _dbContext.Customers.Include(c => c.Address)
                                             .FirstOrDefaultAsync(c => c.Id == id && !c.Disabled);
        }


        public async Task<Customer> FindByCreditCardIdAsync(int creditCardId)
        {
            return await _dbContext.Customers.Include(c => c.Address)
                                             .Where(c => c.CreditCards.FirstOrDefault().Id == creditCardId && !c.Disabled)
                                             .FirstOrDefaultAsync();
        }

        public async Task<bool> CustomerExistsAsync(Customer customer)
        {
            return await _dbContext.Customers.AnyAsync(c => c.FirstName.ToLower() == customer.FirstName.ToLower()
                                                            && c.LastName.ToLower() == customer.LastName.ToLower()
                                                            && c.FatherName.ToLower() == customer.FatherName.ToLower());
        }


        public async Task<Customer> AddAsync(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);

            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            var existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id && !c.Disabled);

            if (existingCustomer != null)
            {
                _dbContext.Entry(existingCustomer).State = EntityState.Detached;
            }

            _dbContext.Entry(customer).State = EntityState.Modified;

            return customer;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer != null)
            {
                customer.Disabled = true;

                isDeleted = true;
            }

            return isDeleted;
        }
    }
}
