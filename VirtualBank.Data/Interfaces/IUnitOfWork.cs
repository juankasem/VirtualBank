using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace VirtualBank.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAddressRepository Addresses { get; }

        IBankAccountRepository BankAccounts { get; }

        IBranchRepository Branches { get; }

        ICashTransactionsRepository CashTransactions { get; }

        ICitiesRepository Cities { get; }

        ICountriesRepository Countries { get; }

        ICustomerRepository Customers { get; }

        ICreditCardsRepository CreditCards { get; }

        IDebitCardsRepository DebitCards { get; }

        IDistrictsRepository Districts { get; }

        IFastTransactionsRepository FastTransactions { get; }

        ILoansRepository Loans { get; }

        IUtilityPaymentRepository UtilityPayments { get; }

        IDbContextTransaction CreateTransaction();

        Task<IDbContextTransaction> CreateTransactionAsync();

        Task<int> SaveAsync();

        Task<int> CompleteTransactionAsync();
    }
}
