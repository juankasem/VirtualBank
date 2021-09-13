using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VirtualBankDbContext _dbContext;
        private IDbContextTransaction _dbContextTransaction;


        public IAddressRepository Addresses { get; private set; }

        public IBankAccountRepository BankAccounts { get; private set; }

        public IBranchRepository Branches { get; private set; }

        public ICashTransactionsRepository CashTransactions { get; private set; }

        public ICitiesRepository Cities { get; private set; }

        public ICountriesRepository Countries { get; private set; }

        public ICustomerRepository Customers { get; private set; }

        public ICreditCardsRepository CreditCards { get; private set; }

        public IDebitCardsRepository DebitCards { get; private set; }

        public IDistrictsRepository Districts { get; private set; }

        public IFastTransactionsRepository FastTransactions { get; private set; }

        public ILoansRepository Loans { get; private set; }

        public IUtilityPaymentRepository UtilityPayments { get; private set; }


        public UnitOfWork(VirtualBankDbContext dbContext)
        {
            _dbContext = dbContext;

            Addresses ??= new AddressRepository(_dbContext);
            BankAccounts ??= new BankAccountRepository(_dbContext);
            Branches ??= new BranchRepository(_dbContext);
            CashTransactions ??= new CashTransactionsRepository(_dbContext);
            Cities ??= new CitiesRepository(_dbContext);
            Countries ??= new CountriesRepository(_dbContext);
            Customers ??= new CustomerRepository(_dbContext);
            CreditCards ??= new CreditCardsRepository(_dbContext);
            DebitCards ??= new DebitCardsRepository(_dbContext);
            FastTransactions ??= new FastTransactionsRepository(_dbContext);
            Loans ??= new LoansRepository(_dbContext);
            UtilityPayments ??= new UtilityPaymentRepository(_dbContext);
        }

        public IDbContextTransaction CreateTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> CreateTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }


        public async Task<int> CompleteTransactionAsync()
        {
            using (_dbContextTransaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var affected = await _dbContext.SaveChangesAsync();
                    await _dbContextTransaction.CommitAsync();

                    return affected;
                }
                catch
                {
                    await _dbContextTransaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async void Dispose()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
