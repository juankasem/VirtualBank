using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class CashTransactionsService : ICashTransactionsService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;

        public CashTransactionsService(VirtualBankDbContext dbContext,
                                     UserManager<AppUser> userManager,
                                     ICustomerService customersService,
                                     IAccountService accountsService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _customerService = customersService;
            _accountService = accountsService;
        }

        /// <summary>
        /// retrieve all transactions that occured in this ccount (from or to account)
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionsResponse>> GetCashTransactionsByAccountNoAsync(string accountNo, int lastDays, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionsResponse>();

            var cashTransactionsList = await _dbContext.CashTransactions.Where(c => (c.From == accountNo || c.To == accountNo)
                                                                               && DateTime.UtcNow.Subtract(c.CreatedOn).TotalDays <= lastDays)
                                                                                 .AsNoTracking().ToListAsync();

            if(cashTransactionsList.Count() == 0) {
                return responseModel;
            }

            var cashTransactions = new ImmutableArray<CashTransactionResponse>();

            foreach (var cashTransaction in cashTransactionsList)
            {
                if (cashTransaction.From != accountNo && cashTransaction.Type == CashTransactionType.Transfer)
                {
                    var senderAccount = await _customerService.GetCustomerByAccountNoAsync(cashTransaction.From, cancellationToken);
                    var sender = senderAccount?.Data?.Customer.FirstName + " " + senderAccount?.Data?.Customer.LastName;

                    cashTransactions.Add(new CashTransactionResponse(cashTransaction, sender, accountNo));
                }

                else if(cashTransaction.To != accountNo && cashTransaction.Type == CashTransactionType.Transfer)
                {
                    var recipientAccount = await _customerService.GetCustomerByAccountNoAsync(cashTransaction.To, cancellationToken);
                    var recipient = recipientAccount?.Data?.Customer.FirstName + " " + recipientAccount?.Data?.Customer.LastName;

                    cashTransactions.Add(new CashTransactionResponse(cashTransaction, accountNo, recipient));
                }

                else if (cashTransaction.Type == CashTransactionType.Deposit || cashTransaction.Type == CashTransactionType.Withdrawal)
                {
                    cashTransactions.Add(new CashTransactionResponse(cashTransaction, cashTransaction.From, cashTransaction.To));
                }
            }

            responseModel.Data = new CashTransactionsResponse(cashTransactions);


            return responseModel;
        }

        public Task<ApiResponse<CashTransactionsResponse>> GetCashTransactionsByCustomerNoAsync(string customerNo, int lastDays, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a cash transaction in db
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddCashTransactionAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();


            using var dbContextTransaction = _dbContext.Database.BeginTransaction();

            switch (request.CashTransaction.Type)
            {
                case CashTransactionType.Deposit:
                    try
                    {
                        var toAccountResponse = await _accountService.GetAccountByAccountNoAsync(request.CashTransaction.To, cancellationToken);

                        if (toAccountResponse?.Data == null)
                        {
                            responseModel.AddError(toAccountResponse.Errors[0]);
                            return responseModel;
                        }

                        var toAccount = toAccountResponse.Data.Account;

                        toAccount.Balance += request.CashTransaction.Amount;
                        await _dbContext.SaveChangesAsync();

                        var cashTransaction = InstantiateCashTransaction(request);

                        await _dbContext.CashTransactions.AddAsync(cashTransaction);
                        await _dbContext.SaveChangesAsync();

                        dbContextTransaction.Commit();

                        return responseModel;
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        responseModel.AddError(ex.ToString());

                        return responseModel;
                    }


                case CashTransactionType.Withdrawal:
                    try
                    {
                        var fromAccountResponse = await _accountService.GetAccountByAccountNoAsync(request.CashTransaction.From, cancellationToken);

                        if (fromAccountResponse?.Data == null)
                        {
                            responseModel.AddError(fromAccountResponse.Errors[0]);
                            return responseModel;
                        }

                        var fromAccount = fromAccountResponse.Data.Account;

                        if (request.CashTransaction.Amount <= fromAccount.Balance)
                        {
                            fromAccount.Balance -= request.CashTransaction.Amount;
                            await _dbContext.SaveChangesAsync();

                            var cashTransaction = InstantiateCashTransaction(request);

                            await _dbContext.CashTransactions.AddAsync(cashTransaction);
                            await _dbContext.SaveChangesAsync();

                            dbContextTransaction.Commit();

                            return responseModel;
                        }
                        else
                        {
                            responseModel.AddError("Balance is not enough to complete withdrawal");
                            return responseModel;
                        }
                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        responseModel.AddError(ex.ToString());

                        return responseModel;
                    }

                case CashTransactionType.Transfer:

                    var senderAccountResponse = await _accountService.GetAccountByAccountNoAsync(request.CashTransaction.From, cancellationToken);
                    var recipientAccountResponse = await _accountService.GetAccountByAccountNoAsync(request.CashTransaction.To, cancellationToken);


                    if (senderAccountResponse?.Data == null)
                    {
                        responseModel.AddError(senderAccountResponse.Errors[0]);
                        return responseModel;
                    }

                    if (recipientAccountResponse?.Data == null)
                    {
                        responseModel.AddError(recipientAccountResponse.Errors[0]);
                        return responseModel;
                    }

                    var senderAccount = senderAccountResponse.Data.Account;
                    var recipientAccount = recipientAccountResponse.Data.Account;


                    if (request.CashTransaction.Amount <= senderAccount.Balance)
                    {
                        senderAccount.Balance -= request.CashTransaction.Amount;
                        await _dbContext.SaveChangesAsync();

                        recipientAccount.Balance += request.CashTransaction.Amount;
                        await _dbContext.SaveChangesAsync();

                        var cashTransaction = InstantiateCashTransaction(request);

                        await _dbContext.CashTransactions.AddAsync(cashTransaction);
                        await _dbContext.SaveChangesAsync();

                        dbContextTransaction.Commit();

                        return responseModel;
                    }
                    else
                    {
                        responseModel.AddError("No enough balance to complete transfer");
                        return responseModel;
                    }

                default:
                    return responseModel;
            }

        }

        /// <summary>
        /// delete a transaction from db
        /// </summary>
        /// <param name="cashTransactionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        public async Task<ApiResponse> DeleteCashTransactionAsync(string cashTransactionId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        private CashTransaction InstantiateCashTransaction(CreateCashTransactionRequest request)
        {
          return  new CashTransaction()
          {
                Type = request.CashTransaction.Type,
                From = request.CashTransaction.From,
                To = request.CashTransaction.To,
                Amount = request.CashTransaction.Amount,
                Description = request.CashTransaction.Description,
                Status = TransactionStatusType.Completed
          };
        }
    }
}
