using System;
using System.Collections.Generic;
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
        private readonly ICustomerService _customerService;
        private readonly IBankAccountService _bankAccountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashTransactionsService(VirtualBankDbContext dbContext,
                                     ICustomerService customersService,
                                     IBankAccountService bankAccountsService,
                                     IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _customerService = customersService;
            _bankAccountService = bankAccountsService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// retrieve all transactions that occured in the specified account(from or to)
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionListResponse>> GetCashTransactionsByIBANAsync(string iban,
                                                                                                   int lastDays,
                                                                                                   int pageNumber,
                                                                                                   int pageSize,
                                                                                                   CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionListResponse>();
            var skip = (pageNumber - 1) * pageSize;

            var cashTransactionList = await _dbContext.CashTransactions.Where(c => (c.From == iban || c.To == iban)
                                                                               && DateTime.UtcNow.Subtract(c.TransactionDate).TotalDays <= lastDays)
                                                                                 .Skip(skip).Take(pageSize)
                                                                                 .AsNoTracking().ToListAsync();

            if(cashTransactionList.Count() == 0) {
                return responseModel;
            }

            var cashTransactions = new List<CashTransactionResponse>();

            foreach (var cashTransaction in cashTransactionList)
            {
                if (cashTransaction.From != iban && cashTransaction.Type == CashTransactionType.Transfer)
                {
                    var senderResponse = await _customerService.GetCustomerByIBANAsync(cashTransaction.From, cancellationToken);
                    var sender = senderResponse?.Data?.FullName;

                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, sender, iban, Direction.In));
                }

                else if(cashTransaction.To != iban && cashTransaction.Type == CashTransactionType.Transfer)
                {
                    var recipientResponse = await _customerService.GetCustomerByIBANAsync(cashTransaction.To, cancellationToken);
                    var recipient = recipientResponse?.Data?.FullName;

                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, iban, recipient, Direction.Out));
                }

                else if (cashTransaction.Type == CashTransactionType.Deposit)
                {
                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, "", cashTransaction.To, Direction.In));
                }
                else if (cashTransaction.Type == CashTransactionType.Withdrawal)
                {
                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, cashTransaction.From, "", Direction.Out));
                }
                else if (cashTransaction.Type == CashTransactionType.CommissionFees)
                {
                    cashTransactions.Add(CreateCashTransactionResponse(cashTransaction, cashTransaction.From, "", Direction.Out));
                }
            }

            responseModel.Data = new CashTransactionListResponse(cashTransactions.ToImmutableList(), cashTransactions.Count);

            return responseModel;
        }

        public Task<ApiResponse<CashTransactionListResponse>> GetFastCashTransactionsByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
                                                                   .OrderByDescending(c => c.TransactionDate).FirstOrDefaultAsync();
                                                                             
            if (lastTransaction == null)
            {
                return responseModel;
            }


            responseModel.Data = CreateCashTransactionResponse(lastTransaction, lastTransaction.From, lastTransaction.To, Direction.In);

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

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            using (dbContextTransaction)
            {
                switch (request.CashTransaction.Type)
                {
                    case CashTransactionType.Deposit:
                        try
                        {
                            var toAccount = await GetBankAccountByIBANAsync(request.CashTransaction.To);

                            if (toAccount == null)
                            {
                                responseModel.AddError("account not found");
                                return responseModel;
                            }

                            //Add amount to recipient balance
                            toAccount.Balance += request.CashTransaction.Amount;

                            _dbContext.BankAccounts.Update(toAccount);
                            await _dbContext.SaveChangesAsync();

                            var cashTransaction = CreateCashTransaction(request, username, 0, toAccount.Balance);

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
                            var fromAccount = await GetBankAccountByIBANAsync(request.CashTransaction.From);

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

                                var cashTransaction = CreateCashTransaction(request, username, fromAccount.Balance, 0);


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
                            var senderAccount = await GetBankAccountByIBANAsync(request.CashTransaction.From);
                            var recipientAccount = await GetBankAccountByIBANAsync(request.CashTransaction.To);
                            var amount = request.CashTransaction.Amount;

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

                            if (amount <= senderAccount.Balance)
                            {
                                const double FeesRate = 0.0015;

                                var fees = (double)amount * FeesRate;

                                //Deduct from sender account
                                senderAccount.Balance -= amount;

                                _dbContext.BankAccounts.Update(senderAccount);
                                await _dbContext.SaveChangesAsync();

                                //Deposit to recipient account
                                recipientAccount.Balance += amount;

                                _dbContext.BankAccounts.Update(recipientAccount);
                                await _dbContext.SaveChangesAsync();

                                //Create a transaction 
                                var cashTransaction = CreateCashTransaction(request, username, senderAccount.Balance, recipientAccount.Balance);

                                //Save transaction into db
                                await _dbContext.CashTransactions.AddAsync(cashTransaction);
                                await _dbContext.SaveChangesAsync();

                                //Deduct commission fees from sender account
                                senderAccount.Balance -= (decimal)fees;

                                _dbContext.BankAccounts.Update(senderAccount);
                                await _dbContext.SaveChangesAsync();

                                request.CashTransaction.Type = CashTransactionType.CommissionFees;
                                request.CashTransaction.Amount = (decimal)fees;

                                //Create a transaction for the comission fees
                                var commissionFeeTransaction = CreateCashTransaction(request, username, senderAccount.Balance, 0);

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

        }


        #region private Helper methods
        [NonAction]
        private CashTransaction CreateCashTransaction(CreateCashTransactionRequest request, string username, decimal senderBalance, decimal recipientBalance)
        {
            const string BANKACCOUNTNO = "000100000";
            var cashTransaction = request.CashTransaction;
            var isTransferFees = cashTransaction.Type == CashTransactionType.CommissionFees;

          return  new CashTransaction()
          {
            Type = cashTransaction.Type,
            From = cashTransaction.From,
            To = !isTransferFees ? cashTransaction.To : BANKACCOUNTNO,
            Amount = cashTransaction.Amount,
            SenderRemainingBalance = senderBalance,
            RecipientRemainingBalance = recipientBalance,
            InitiatedBy = cashTransaction.InitiatedBy,
            Description = cashTransaction.Description,                                          
            PaymentType = !isTransferFees ? cashTransaction.PaymentType : PaymentType.ComissionFees,
            CreatedBy = username,
            TransactionDate = cashTransaction.TransactionDate, 
            Status = cashTransaction.TransactionDate == DateTime.Today ? TransactionStatusType.Completed : TransactionStatusType.Pending
          };

        }

        [NonAction]
        private CashTransactionResponse CreateCashTransactionResponse(CashTransaction cashTransaction, string sender, string recipient, Direction direction)                        
        {
            var isTransferFees = cashTransaction.Type == CashTransactionType.CommissionFees;

            return new CashTransactionResponse(cashTransaction.From,
                                               cashTransaction.To,
                                               direction == Direction.In ? cashTransaction.Amount : -cashTransaction.Amount,
                                               sender,
                                               recipient,
                                               cashTransaction.PaymentType,
                                               isTransferFees ? "Transafer comission Fees" :
                                               direction == Direction.In ?
                                                $"From: {sender}, Account No: {cashTransaction.From} "
                                                :
                                                cashTransaction.InitiatedBy == BankAssetType.POS || cashTransaction.InitiatedBy == BankAssetType.ATM ?
                                                $"{cashTransaction.InitiatedBy} purchase: {recipient}"
                                                :
                                                $"{cashTransaction.To}--{recipient}, {cashTransaction.Description}", 
                                               cashTransaction.InitiatedBy,
                                               direction == Direction.In ? cashTransaction.RecipientRemainingBalance : cashTransaction.SenderRemainingBalance,
                                               cashTransaction.CreatedOn,
                                               cashTransaction.CreatedBy);
        }

        [NonAction]
        private async Task<BankAccount> GetBankAccountByIBANAsync(string iban)
        {
          return  await _dbContext.BankAccounts.FirstOrDefaultAsync(c => c.IBAN == iban);
        }

        #endregion
    }
}
