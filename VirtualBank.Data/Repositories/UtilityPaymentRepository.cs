using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class UtilityPaymentRepository : IUtilityPaymentRepository
    {
        private readonly VirtualBankDbContext _dbContext;

        public UtilityPaymentRepository(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<UtilityPayment>> ListAllAsync()
        {

            return await _dbContext.UtilityPayments.Include(b => b.BankAccount)
                                                   .ThenInclude(c => c.Owner)
                                                   .Where(l => !l.Disabled)
                                                   .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<UtilityPayment>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbContext.UtilityPayments.Include(b => b.BankAccount)
                                                   .ThenInclude(c => c.Owner)
                                                   .Where(c => c.BankAccount.CustomerId == customerId && !c.Disabled)
                                                   .AsNoTracking().ToListAsync();
        }

        public async Task<UtilityPayment> FindByIdAsync(int id)
        {
            return await _dbContext.UtilityPayments.Include(b => b.BankAccount)
                                                   .ThenInclude(c => c.Owner)
                                                   .FirstOrDefaultAsync(c => c.Id == id && !c.Disabled);
        }

        public async Task<UtilityPayment> AddAsync(UtilityPayment utilityPayment)
        {
            await _dbContext.UtilityPayments.AddAsync(utilityPayment);

            return utilityPayment;
        }

        public async Task<UtilityPayment> UpdateAsync(UtilityPayment utilityPayment)
        {
            var existingUtilityPayment = await _dbContext.UtilityPayments.FirstOrDefaultAsync(u => u.Id == utilityPayment.Id && !u.Disabled);

            if (existingUtilityPayment != null)
            {
                _dbContext.Entry(existingUtilityPayment).State = EntityState.Detached;
            }

            _dbContext.Entry(utilityPayment).State = EntityState.Modified;

            return utilityPayment;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var isDeleted = false;
            var utilityPayment = await _dbContext.UtilityPayments.FindAsync(id);

            if (utilityPayment != null)
            {
                utilityPayment.Disabled = true;

                isDeleted = true;
            }

            return isDeleted;
        }
    }
}
