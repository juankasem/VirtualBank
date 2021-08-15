using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Models;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CashTransactionsService : ICashTransactionsService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly ICustomerRepository _customerRepo;
        private readonly IBankAccountRepository _bankAccountRepo;
        private readonly ICashTransactionsRepository _cashTransactionsRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashTransactionsService(VirtualBankDbContext dbContext,
                                       ICustomerRepository customersRepo,
                                       IBankAccountRepository bankAccountRepo,
                                       ICashTransactionsRepository cashTransactionsRepo,
                                       IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _customerRepo = customersRepo;
            _bankAccountRepo = bankAccountRepo;
            _cashTransactionsRepo = cashTransactionsRepo;
            _httpContextAccessor = httpContextAccessor;
        }


        /// <summary>
        /// Retrieve all cash transactions occured
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionListResponse>> GetAllCashTransactionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionListResponse>();

            var allCashTransactions = await _cashTransactionsRepo.GetAllAsync();

            if (!allCashTransactions.Any())
            {
                return responseModel;
            }

            var cashTransactions = allCashTransactions.OrderByDescending(c => c.CreatedAt).Skip((pageNumber - 1) * pageSize)
                                                                                           .Take(pageSize);

            var cashTransactionList = new List<CashTransactionResponse>();

            foreach (var cashTransaction in cashTransactions)
            {
                var sender = await GetCustomerName(cashTransaction.From);
                var recipient = await GetCustomerName(cashTransaction.To);

                cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, cashTransaction.From, sender, recipient));
            }

            responseModel.Data = new CashTransactionListResponse(cashTransactionList.ToImmutableList(), cashTransactionList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve all cash transactions that occured in the specified account(from or to it)
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="lastDays"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionListResponse>> GetBankAccountCashTransactionsAsync(string iban, int lastDays,
                                                                                                        int pageNumber, int pageSize,
                                                                                                        CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionListResponse>();

            var accountHolder = await _customerRepo.FindByIBANAsync(iban);
            var accountHolderName = accountHolder.FirstName + " " + accountHolder.LastName;


            var accountCashTransactions = await _cashTransactionsRepo.GetByIBANAsync(iban, lastDays);

            if (!accountCashTransactions.Any())
            {
                return responseModel;
            }

            var cashTransactions = accountCashTransactions.OrderByDescending(c => c.CreatedAt).Skip((pageNumber - 1) * pageSize)
                                                                                              .Take(pageSize);

            var cashTransactionList = new List<CashTransactionResponse>();

            foreach (var cashTransaction in cashTransactions)
            {
                if (cashTransaction.From != iban && IsTransferTransaction(cashTransaction))
                {
                    var sender = await GetCustomerName(cashTransaction.From);

                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, sender, accountHolderName));
                }

                else if (cashTransaction.To != iban && IsTransferTransaction(cashTransaction))
                {
                    var recipient = await GetCustomerName(cashTransaction.To);

                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderName, recipient));
                }

                else if (cashTransaction.Type == CashTransactionType.Deposit)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, Enum.GetName(typeof(BankAssetType), cashTransaction.InitiatedBy), accountHolderName));
                }
                else if (cashTransaction.Type == CashTransactionType.Withdrawal)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderName, Enum.GetName(typeof(BankAssetType), cashTransaction.InitiatedBy)));
                }
                else if (cashTransaction.Type == CashTransactionType.CommissionFees)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderName, "commission fees"));
                }
            }

            responseModel.Data = new CashTransactionListResponse(cashTransactionList.ToImmutableList(), cashTransactionList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrive last cash transactions of the specified iban
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<LastCashTransactionListResponse>> GetLatestTransfersAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LastCashTransactionListResponse>();

            var latestTransfers = await _cashTransactionsRepo.GetLastByIBANAsync(iban);

            if (!latestTransfers.Any())
            {
                return responseModel;
            }

            var latestTransfersList = latestTransfers.OrderByDescending(c => c.CreatedAt).Where(c => IsTransferTransaction(c)).Take(7)
                                                     .Select(async tx => CreateLatestTransferResponse(tx.To, await GetCustomerName(tx.To), new Amount(tx.Amount), tx.TransactionDate))
                                                     .Select(t => t.Result)
                                                     .ToImmutableList();


            responseModel.Data = new LastCashTransactionListResponse(latestTransfersList, latestTransfersList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve last transaction that occured the specified account(from or to)
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionResponse>> GetLastCashTransactionAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();

            var lastTransaction = await _cashTransactionsRepo.FindByIBANAsync(iban);

            if (lastTransaction == null)
            {
                return responseModel;
            }


            responseModel.Data = CreateCashTransactionResponse(lastTransaction, iban, "", "");

            return responseModel;
        }


        /// <summary>
        /// Create a deposit transaction in db
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionResponse>> MakeDepositAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToDeposit = request.DebitedFunds.Amount;
            var currency = request.DebitedFunds.Currency;

            using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var toAccount = await _bankAccountRepo.FindByIBANAsync(request.To);

                if (toAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(toAccount)));
                    return responseModel;
                }

                //Add amount to recipient balance
                toAccount.Balance.Add(amountToDeposit);
                toAccount.AllowedBalanceToUse.Add(amountToDeposit);

                //Update bank account
                await _bankAccountRepo.UpdateAsync(toAccount, _dbContext);

                //Add transaction to db & save changes 
                var createdTx = await _cashTransactionsRepo.AddAsync(CreateCashTransaction(request, 0, toAccount.Balance), _dbContext);

                var depositor = await GetCustomerName(request.To);
                var initiatedBy = GetInitiatedBy(request.InitiatedBy);

                responseModel.Data = CreateCashTransactionResponse(createdTx, request.To, initiatedBy, depositor);

                await dbContextTransaction.CommitAsync(cancellationToken);

                return responseModel;
            }

            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                return responseModel;
            }
        }


        /// <summary>
        /// Create a withdrawal transaction in db
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionResponse>> MakeWithdrawalAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToWithdraw = request.DebitedFunds.Amount;
            var currency = request.DebitedFunds.Currency;

            using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var fromAccount = await _bankAccountRepo.FindByIBANAsync(request.From);
                bool isBlocked = true;

                if (fromAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(fromAccount)));
                    return responseModel;
                }

                if (fromAccount.Type == AccountType.Savings)
                {
                    var deposits = await _cashTransactionsRepo.GetDepositsByIBANAsync(fromAccount.IBAN);

                    foreach (var deposit in deposits)
                    {
                        if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 180)
                        {
                            isBlocked = false;
                        }
                    }

                    if (isBlocked)
                    {
                        responseModel.AddError(ExceptionCreator.CreateUnprocessableEntityError(nameof(fromAccount), "deposit is blocked, you cannot complete withdrawal"));
                        return responseModel;
                    }
                }

                if (amountToWithdraw <= fromAccount.AllowedBalanceToUse)
                {
                    fromAccount.Balance.Subtract(amountToWithdraw);
                    fromAccount.AllowedBalanceToUse.Subtract(amountToWithdraw);

                    await _bankAccountRepo.UpdateAsync(fromAccount, _dbContext);

                    var createdTransaction = await _cashTransactionsRepo.AddAsync(CreateCashTransaction(request, fromAccount.Balance, 0), _dbContext);

                    var withdrawer = await GetCustomerName(request.From);
                    var initiatedBy = GetInitiatedBy(request.InitiatedBy);

                    responseModel.Data = CreateCashTransactionResponse(createdTransaction, request.From, withdrawer, initiatedBy);

                    await dbContextTransaction.CommitAsync();

                    return responseModel;
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(fromAccount), "balance is not enough to complete withdrawal"));

                    return responseModel;
                }
            }

            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync();
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                return responseModel;
            }

        }


        /// <summary>
        /// Create a Transfer transaction in db (between acoounts in the same bank)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionResponse>> MakeTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToTransfer = request.DebitedFunds.Amount;
            var currency = request.DebitedFunds.Currency;

            using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var senderAccount = await _bankAccountRepo.FindByIBANAsync(request.From);
                var recipientAccount = await _bankAccountRepo.FindByIBANAsync(request.To);

                if (senderAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(senderAccount), "sender account not found"));
                    return responseModel;
                }

                if (recipientAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(recipientAccount), "recipient account not found"));
                    return responseModel;
                }

                if (senderAccount.IBAN == recipientAccount.IBAN)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError("sender should send to a different bad account"));
                    return responseModel;

                }

                if (senderAccount.Type != AccountType.Current || senderAccount.Type != AccountType.Recurring)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(senderAccount), $"transaction is not allowed, {Enum.GetName(typeof(AccountType), senderAccount.Type)} account type "));
                    return responseModel;
                }

                if (currency != senderAccount.Currency.Code)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(currency), $"Funds currency should match the correny of the bank account"));
                    return responseModel;
                }

                if (currency != senderAccount.Currency.Code)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(currency), $"Funds currency should match the correny of the bank account"));
                    return responseModel;
                }


                if (amountToTransfer <= senderAccount.AllowedBalanceToUse)
                {
                    //Deduct from sender account
                    senderAccount.Balance.Subtract(amountToTransfer);
                    senderAccount.AllowedBalanceToUse.Subtract(amountToTransfer);

                    //Update sender bank account
                    await _bankAccountRepo.UpdateAsync(senderAccount, _dbContext);

                    //Deposit to recipient account
                    recipientAccount.Balance.Add(amountToTransfer);
                    senderAccount.AllowedBalanceToUse.Add(recipientAccount.Balance);

                    //Update recipient bank account
                    await _bankAccountRepo.UpdateAsync(recipientAccount, _dbContext);

                    //Create & Save transaction into db
                    var createdTransaction = await _cashTransactionsRepo.AddAsync(CreateCashTransaction(request, senderAccount.Balance, recipientAccount.Balance), _dbContext);

                    var sender = await GetCustomerName(request.From);
                    var recipient = await GetCustomerName(request.To);

                    responseModel.Data = CreateCashTransactionResponse(createdTransaction, request.From, sender, recipient);

                    await dbContextTransaction.CommitAsync(cancellationToken);

                    return responseModel;
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError("transfer", "not enough balance to complete transfer"));

                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                return responseModel;
            }
        }


        /// <summary>
        /// Create an EFT Transfer transaction in db (from/to acoounts in different banks)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionResponse>> MakeEFTTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToTransfer = request.DebitedFunds.Amount;
            var currency = request.DebitedFunds.Currency;

            using var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var senderAccount = await _bankAccountRepo.FindByIBANAsync(request.From);

                if (senderAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(senderAccount), "sender account not found"));
                      return responseModel;
                }

                var recipientAccount = await _bankAccountRepo.FindByIBANAsync(request.To);

                if (recipientAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(recipientAccount), "recipient account not found"));
                      return responseModel;
                }

                if (senderAccount.IBAN == recipientAccount.IBAN)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError("sender should send to a different bad account"));
                    return responseModel;

                }

                if (recipientAccount.Owner.FirstName != request.RecipientFirstName || recipientAccount.Owner.LastName != request.RecipientLastName)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(recipientAccount), "recipient name does not match account holder's name"));
                      return responseModel;
                }

                if (senderAccount.Type != AccountType.Current || senderAccount.Type != AccountType.Recurring)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(senderAccount), $"transaction is not allowed, {Enum.GetName(typeof(AccountType), senderAccount.Type)} account type "));
                      return responseModel;
                }

                if (currency != senderAccount.Currency.Code)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(currency), $"Funds currency should match the correny of the bank account"));
                    return responseModel;
                }


                if (amountToTransfer <= senderAccount.AllowedBalanceToUse)
                {
                    const decimal feesRate = (decimal) 0.0015;
                    var fees = new Amount(amountToTransfer * feesRate);

                    //Deduct from sender account
                    senderAccount.Balance.Subtract(amountToTransfer);
                    senderAccount.AllowedBalanceToUse.Subtract(amountToTransfer);

                    await _bankAccountRepo.UpdateAsync(senderAccount, _dbContext);

                    //Deposit to recipient account
                    recipientAccount.Balance.Add(amountToTransfer);
                    recipientAccount.AllowedBalanceToUse.Add(amountToTransfer);

                    await _bankAccountRepo.UpdateAsync(recipientAccount, _dbContext);


                    //Create & Save transaction into db
                    var createdTransaction = await _cashTransactionsRepo.AddAsync(CreateCashTransaction(request, senderAccount.Balance, recipientAccount.Balance, fees), _dbContext);

                    var sender = await GetCustomerName(request.From);
                    var recipient = await GetCustomerName(request.To);

                    responseModel.Data = CreateCashTransactionResponse(createdTransaction, request.From, sender, recipient);

                    //Deduct commission fees from sender account
                    senderAccount.Balance.Subtract(fees);
                    senderAccount.AllowedBalanceToUse.Subtract(fees);

                    //Update entity & Save it to db
                    await _bankAccountRepo.UpdateAsync(senderAccount, _dbContext);

                    //Modify request for commission fees transaction
                    request.Type = CashTransactionType.CommissionFees;
                    request.DebitedFunds = CreateDebitedFunds(fees, request.DebitedFunds.Currency);

                    //Create & Save commission fees into db
                    await _cashTransactionsRepo.AddAsync(CreateCashTransaction(request, senderAccount.Balance, 0), _dbContext);

                    await dbContextTransaction.CommitAsync(cancellationToken);

                    return responseModel;
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError("transfer", "not enough balance to complete transfer"));
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                return responseModel;
            }
        }


        #region private Helper methods
        [NonAction]
        private CashTransaction CreateCashTransaction(CreateCashTransactionRequest request, decimal senderBalance, decimal recipientBalance, decimal fees = 0)
        {
            const string BANKACCOUNTNO = "000100000";
            var isTransferFees = request.Type == CashTransactionType.CommissionFees;

            return new CashTransaction()
            {
                Type = request.Type,
                From = request.From,
                To = !isTransferFees ? request.To : BANKACCOUNTNO,
                Amount = request.DebitedFunds.Amount,
                Fees = fees > 0 ? fees : 0,
                Currency = request.DebitedFunds.Currency,
                SenderRemainingBalance = senderBalance,
                RecipientRemainingBalance = recipientBalance,
                InitiatedBy = request.InitiatedBy,
                Description = request.Description,
                PaymentType = !isTransferFees ? request.PaymentType : PaymentType.CommissionFees,
                CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name,
                TransactionDate = request.TransactionDate,
                CreditCardNo = request.CreditCardNo,
                DebitCardNo = request.DebitCardNo
            };
        }

        [NonAction]
        private static CashTransactionResponse CreateCashTransactionResponse(CashTransaction cashTransaction, string iban, string sender, string recipient)
        {
            var isTransferFees = cashTransaction.Type == CashTransactionType.CommissionFees;
        

            return new CashTransactionResponse(cashTransaction.From,
                                               cashTransaction.To,
                                               cashTransaction.From != iban ? CreateDebitedFunds(cashTransaction.Amount, cashTransaction.Currency)
                                               : CreateDebitedFunds(-cashTransaction.Amount, cashTransaction.Currency),
                                               sender,
                                               recipient,
                                               cashTransaction.PaymentType,
                                               isTransferFees ? "Transfer commission Fees" :
                                                cashTransaction.From != iban ?
                                                $"From: {sender}, Account No: {cashTransaction.From} "
                                                :
                                                cashTransaction.InitiatedBy == BankAssetType.POS ? cashTransaction.DebitCardNo != null ?
                                                $"{cashTransaction.InitiatedBy} purchase: card No: {cashTransaction.DebitCardNo}, {recipient}"
                                                :
                                                $"{cashTransaction.InitiatedBy} purchase: card No: {cashTransaction.CreditCardNo}, {recipient}"
                                                :
                                                $"{cashTransaction.To}--{recipient}, {cashTransaction.Description}",
                                                cashTransaction.InitiatedBy,
                                                cashTransaction.From != iban ? cashTransaction.RecipientRemainingBalance : cashTransaction.SenderRemainingBalance,
                                                cashTransaction.CreatedAt,
                                                cashTransaction.CreatedBy);
        }


        [NonAction]
        private static LastCashTransactionResponse CreateLatestTransferResponse(string toAccount, string recipient, Amount amount, DateTime createdOn)
        {
            return new LastCashTransactionResponse(toAccount, recipient, amount, createdOn);
        }

        private static DebitedFunds CreateDebitedFunds(decimal amount, string currency)
        {
            return new DebitedFunds(new Amount(amount), currency);
        }


        [NonAction]
        private async Task<string> GetCustomerName(string iban)
        {
            var customer = await _customerRepo.FindByIBANAsync(iban);
            var customerFullName = customer.FirstName + " " + customer.LastName;

            return customerFullName;
        }


        [NonAction]
        private static bool IsTransferTransaction(CashTransaction cashTransaction)
        {
            return cashTransaction.Type == CashTransactionType.Transfer || cashTransaction.Type == CashTransactionType.EFT;
        }


        private static string GetInitiatedBy(BankAssetType bankAssetType)
        {
            switch (bankAssetType)
            {
                case BankAssetType.ATM: return "ATM";

                case BankAssetType.POS: return "POS";

                case BankAssetType.Account: return "ACCOUNT";

                default:
                    return "";
            }
        }

        #endregion
    }
}
