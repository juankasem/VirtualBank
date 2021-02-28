using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;
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
        private readonly IBankAccountService _bankAccountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashTransactionsService(VirtualBankDbContext dbContext,
                                     UserManager<AppUser> userManager,
                                     ICustomerService customersService,
                                     IBankAccountService accountsService,
                                     IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _customerService = customersService;
            _bankAccountService = accountsService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// retrieve all transactions that occured in the specified account(from or to)
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
                    var sender = senderAccount?.Data?.Customer?.FirstName + " " + senderAccount?.Data?.Customer?.LastName;

                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, sender, accountNo));
                }

                else if(cashTransaction.To != accountNo && cashTransaction.Type == CashTransactionType.Transfer)
                {
                    var recipientAccount = await _customerService.GetCustomerByAccountNoAsync(cashTransaction.To, cancellationToken);
                    var recipient = recipientAccount?.Data?.Customer?.FirstName + " " + recipientAccount?.Data?.Customer?.LastName;

                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, accountNo, recipient));
                }

                else if (cashTransaction.Type == CashTransactionType.Deposit || cashTransaction.Type == CashTransactionType.Withdrawal)
                {
                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, cashTransaction.From, cashTransaction.To));
                }
                else if (cashTransaction.Type == CashTransactionType.CommissionFees)
                {
                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, cashTransaction.From, ""));
                }
            }

            responseModel.Data = new CashTransactionsResponse(cashTransactions);


            return responseModel;
        }

        /// <summary>
        /// retrieve last transaction that occured the specified account(from or to)
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionResponse>> GetLastCashTransactionAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();

            var lastTransaction = await _dbContext.CashTransactions.Where(c => c.From == accountNo || c.To == accountNo)
                                                                   .OrderByDescending(c => c.CreatedBy).FirstOrDefaultAsync();
                                                                             
            if (lastTransaction == null)
            {
                return responseModel;
            }

            responseModel.Data = CreateCashTransactionResponse(lastTransaction, lastTransaction.From, lastTransaction.To);

            return responseModel;
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
            var user = _httpContextAccessor.HttpContext.User;
            var username = user.Identity.Name;
            var sender = "";
            var reipient = "";

            await using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            switch (request.CashTransaction.Type)
            {
                case CashTransactionType.Deposit:
                    try
                    {
                        var toAccount = await GetBankAccountByAcccountNoAsync(request.CashTransaction.To);

                        if (toAccount == null)
                        {
                            responseModel.AddError("account not found");
                            return responseModel;
                        }

                        //Add amount to recipient balance
                        toAccount.Balance += request.CashTransaction.Amount;

                        _dbContext.BankAccounts.Update(toAccount);
                        await _dbContext.SaveChangesAsync();

                        var cashTransaction = CreateCashTransaction(request, username, toAccount.Balance);

                        //Add transaction to db & save changes 
                        await _dbContext.CashTransactions.AddAsync(cashTransaction);
                        await _dbContext.SaveChangesAsync();

                        await dbContextTransaction.CommitAsync();

                        return responseModel;
                    }

                    catch (Exception ex)
                    {
                       await dbContextTransaction.RollbackAsync();
                        responseModel.AddError(ex.ToString());

                        return responseModel;
                    }


                case CashTransactionType.Withdrawal:
                    try
                    {
                        var fromAccount = await GetBankAccountByAcccountNoAsync(request.CashTransaction.From);

                        if (fromAccount == null)
                        {
                            responseModel.AddError("account not found");
                            return responseModel;
                        }
                   
                        if (request.CashTransaction.Amount <= fromAccount.Balance)
                        {
                            fromAccount.Balance -= request.CashTransaction.Amount;

                            _dbContext.BankAccounts.Update(fromAccount);
                            await _dbContext.SaveChangesAsync();

                            var cashTransaction = CreateCashTransaction(request, username, fromAccount.Balance);
                            

                            await _dbContext.CashTransactions.AddAsync(cashTransaction);
                            await _dbContext.SaveChangesAsync();

                            await dbContextTransaction.CommitAsync();

                            return responseModel;
                        }
                        else
                        {
                            responseModel.AddError("balance is not enough to complete withdrawal");
                            return responseModel;
                        }
                    }

                    catch (Exception ex)
                    {
                       await dbContextTransaction.RollbackAsync();
                       responseModel.AddError(ex.ToString());

                        return responseModel;
                    }

                case CashTransactionType.Transfer:
                    
                    try
                    {               
                        var senderAccount = await GetBankAccountByAcccountNoAsync(request.CashTransaction.From);
                        var recipientAccount = await GetBankAccountByAcccountNoAsync(request.CashTransaction.To);

                        if (senderAccount == null)
                        {
                            responseModel.AddError("account not found");
                            return responseModel;
                        }

                        if (recipientAccount == null)
                        {
                            responseModel.AddError("account not found");
                            return responseModel;
                        }

                        if (request.CashTransaction.Amount <= senderAccount.Balance)
                        {
                            const double FeesRate = 0.0015;
                            var fees = (double)request.CashTransaction.Amount * FeesRate;

                            //Deduct from sender account
                            senderAccount.Balance -= request.CashTransaction.Amount;

                            _dbContext.BankAccounts.Update(senderAccount);
                            await _dbContext.SaveChangesAsync();

                            //Deposit to recipient account
                            recipientAccount.Balance += request.CashTransaction.Amount;

                            _dbContext.BankAccounts.Update(recipientAccount);
                            await _dbContext.SaveChangesAsync();

                            //Create a transaction 
                            var cashTransaction = CreateCashTransaction(request, username, senderAccount.Balance, );
                    
                            //Save transaction into db
                            await _dbContext.CashTransactions.AddAsync(cashTransaction);
                            await _dbContext.SaveChangesAsync();

                            //Deduct commission fees from sender account
                            senderAccount.Balance -= (decimal)fees;

                            _dbContext.BankAccounts.Update(senderAccount);
                            await _dbContext.SaveChangesAsync();

                            //Create a transaction for the comission fees
                            var commissionFeeTransaction = CreateCashTransaction(request, username, senderAccount.Balance, fees);

                            //Save transaction into db
                            await _dbContext.CashTransactions.AddAsync(commissionFeeTransaction);
                            await _dbContext.SaveChangesAsync();

                            await dbContextTransaction.CommitAsync();


                            return responseModel;
                        }
                        else
                        {
                            responseModel.AddError("No enough balance to complete transfer");
                            return responseModel;
                        }
                    }
                    catch (Exception ex)
                    {
                        await dbContextTransaction.RollbackAsync();
                        responseModel.AddError(ex.ToString());

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

        #region private Helper methods
        [NonAction]
        private CashTransaction CreateCashTransaction(CreateCashTransactionRequest request, string username, decimal remainingBalance,
                                                      string sender = "", string recipient = "", double fees = 0)
        {
          return  new CashTransaction()
          {
            Type = fees == 0 ? request.CashTransaction.Type : CashTransactionType.CommissionFees,
            From = request.CashTransaction.From,
            To = fees == 0 ? request.CashTransaction.To : "",
            Amount = fees == 0 ? request.CashTransaction.Amount : (decimal)fees,
            InitiatedBy = request.CashTransaction.InitiatedBy,
            Description = fees == 0 ? $"From: {sender}, Account No: {request.CashTransaction.From}, " +
                                      $" To: {recipient}, Account No: {request.CashTransaction.To}"
                                      :
                                      "Transafer comission Fee",
            RemainingBalance = remainingBalance,
            PaymentType = fees == 0 ? request.CashTransaction.PaymentType : PaymentType.ComissionFees,
            CreatedBy = username,
            Status = TransactionStatusType.Completed
          };

        }

        [NonAction]
        private CashTransactionResponse CreateCashTransactionResponse(CashTransaction cashTransaction, string sender, string recipient)                        
        {
            return new CashTransactionResponse(cashTransaction.From,
                                               cashTransaction.To,
                                               cashTransaction.Amount,
                                               sender,
                                               recipient,
                                               cashTransaction.PaymentType,
                                               cashTransaction.Description,
                                               cashTransaction.Currency,
                                               cashTransaction.InitiatedBy,
                                               cashTransaction.RemainingBalance,
                                               cashTransaction.CreatedOn,
                                               cashTransaction.CreatedBy);
        }

        [NonAction]
        private async Task<BankAccount> GetBankAccountByAcccountNoAsync(string accountNo)
        {
          return  await _dbContext.BankAccounts.FirstOrDefaultAsync(c => c.AccountNo == accountNo);
        }

        #endregion
    }
}
