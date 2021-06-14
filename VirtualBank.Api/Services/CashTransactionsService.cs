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
                var sender = await _customerRepo.FindByIBANAsync(cashTransaction.From);
                var senderFullName = sender.FirstName + " " + sender.LastName;
                var recipient = await _customerRepo.FindByIBANAsync(cashTransaction.To);
                var recipientFullName = recipient.FirstName + " " + recipient.LastName;

                cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, cashTransaction.From, senderFullName, recipientFullName));

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
            var accountHolderName =  accountHolder.FirstName + " " + accountHolder.LastName;


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
                    var sender = await _customerRepo.FindByIBANAsync(cashTransaction.From);
                    var senderFullName = sender.FirstName + " " + sender.LastName;

                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, senderFullName, accountHolderName));
                }

                else if (cashTransaction.To != iban && IsTransferTransaction(cashTransaction))
                {
                    var recipient = await _customerRepo.FindByIBANAsync(cashTransaction.To);
                    var recipientFullName = recipient.FirstName + " " + recipient.LastName;

                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderName, recipientFullName));
                }

                else if (cashTransaction.Type == CashTransactionType.Deposit)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, "ATM", accountHolderName));
                }
                else if (cashTransaction.Type == CashTransactionType.Withdrawal)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderName, "ATM"));
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
        public async Task<ApiResponse<LastCashTransactionListResponse>> GetLastCashTransactionsAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LastCashTransactionListResponse>();

            var lastCashTransactions = await _cashTransactionsRepo.GetLastByIBANAsync(iban);

            if (!lastCashTransactions.Any())
            {
                return responseModel;
            }

            var cashTransactions = lastCashTransactions.OrderByDescending(c => c.CreatedAt).Take(7);

            var lastCashTransactionList = new List<LastCashTransactionResponse>();

            foreach (var tx in cashTransactions)
            {
                if (IsTransferTransaction(tx))
                {
                    var recipient = await _customerRepo.FindByIBANAsync(tx.To);
                    var recipientFullName = recipient.FirstName + " " + recipient.LastName;

                    lastCashTransactionList.Add(CreateLastCashTransactionResponse(tx.To, recipientFullName, tx.Amount, tx.TransactionDate));
                }
            }

            responseModel.Data = new LastCashTransactionListResponse(lastCashTransactionList.ToImmutableList(), lastCashTransactionList.Count);

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
        public async Task<Response> MakeDepositAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToDeposit = request.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var toAccount = await _bankAccountRepo.FindByIBANAsync(request.To);

                if (toAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(toAccount)));
                    return responseModel;
                }

                //Add amount to recipient balance
                toAccount.Balance += amountToDeposit;
                toAccount.AllowedBalanceToUse += amountToDeposit;

                //Update bank account
                await _bankAccountRepo.UpdateAsync(_dbContext, toAccount);

                //Add transaction to db & save changes 
                await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, 0, toAccount.Balance));

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
        public async Task<Response> MakeWithdrawalAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToWithdraw = request.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
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
                        responseModel.AddError(ExceptionCreator.CreateUnprocessableEntityError(nameof(fromAccount), "deposit is blocked"));
                        return responseModel;
                    }
                }

                if (amountToWithdraw <= fromAccount.AllowedBalanceToUse)
                {
                    fromAccount.Balance -= amountToWithdraw;
                    fromAccount.AllowedBalanceToUse -= amountToWithdraw; 

                    await _bankAccountRepo.UpdateAsync(_dbContext, fromAccount);

                    await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, fromAccount.Balance, 0));

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
        public async Task<Response> MakeTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToTransfer = request.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var senderAccount = await _bankAccountRepo.FindByIBANAsync(request.From);
                var recipientAccount = await _bankAccountRepo.FindByIBANAsync(request.To);

                if (senderAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(senderAccount)));
                    return responseModel;
                }

                if (recipientAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(recipientAccount)));
                    return responseModel;
                }
                
                if (amountToTransfer <= senderAccount.AllowedBalanceToUse)
                {
                    //Deduct from sender account
                    senderAccount.Balance -= amountToTransfer;
                    senderAccount.AllowedBalanceToUse -= amountToTransfer;

                    //Update sender bank account
                    await _bankAccountRepo.UpdateAsync(_dbContext, senderAccount);

                    //Deposit to recipient account
                    recipientAccount.Balance += amountToTransfer;
                    senderAccount.AllowedBalanceToUse = recipientAccount.Balance;

                    //Update recipient bank account
                    await _bankAccountRepo.UpdateAsync(_dbContext, recipientAccount);

                    //Create & Save transaction into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, senderAccount.Balance, recipientAccount.Balance));

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
        public async Task<Response> MakeEFTTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToTransfer = request.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
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

                if (recipientAccount.Owner.FirstName != request.RecipeintFirstName || recipientAccount.Owner.LastName != request.RecipeintLastName)
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(recipientAccount), "recipient name is not valid"));
                    return responseModel;
                }

                if (amountToTransfer <= senderAccount.AllowedBalanceToUse)
                {
                    const double feesRate = 0.0015;
                    var fees = (double)amountToTransfer * feesRate;

                    //Deduct from sender account
                    senderAccount.Balance -= amountToTransfer;
                    senderAccount.AllowedBalanceToUse = senderAccount.Balance;

                    await _bankAccountRepo.UpdateAsync(_dbContext, senderAccount);

                    //Deposit to recipient account
                    recipientAccount.Balance += amountToTransfer;
                    recipientAccount.AllowedBalanceToUse += amountToTransfer;

                    await _bankAccountRepo.UpdateAsync(_dbContext, recipientAccount);


                    //Create & Save transaction into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, senderAccount.Balance, recipientAccount.Balance));

                    //Deduct commission fees from sender account
                    senderAccount.Balance -= (decimal)fees;
                    senderAccount.AllowedBalanceToUse = senderAccount.Balance;

                    //Update entity & Save it to db
                    await _bankAccountRepo.UpdateAsync(_dbContext, senderAccount);

                    //Modify request for commission fees transaction
                    request.Type = CashTransactionType.CommissionFees;
                    request.Amount = (decimal)fees;

                    //Create & Save commission fees into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, senderAccount.Balance, 0));

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
        private CashTransaction CreateCashTransaction(CreateCashTransactionRequest request, decimal senderBalance, decimal recipientBalance)
        {
            const string BANKACCOUNTNO = "000100000";
            var isTransferFees = request.Type == CashTransactionType.CommissionFees;

            return new CashTransaction()
            {
                Type = request.Type,
                From = request.From,
                To = !isTransferFees ? request.To : BANKACCOUNTNO,
                Amount = request.Amount,
                SenderRemainingBalance = senderBalance,
                
                RecipientRemainingBalance = recipientBalance,
                InitiatedBy = request.InitiatedBy,
                Description = request.Description,
                PaymentType = !isTransferFees ? request.PaymentType : PaymentType.ComissionFees,
                CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name,
                TransactionDate = request.TransactionDate,
                CreditCardNo = request.CreditCardNo ?? null,
                DebitCardNo = request.DebitCardNo ?? null
            };
        }

        [NonAction]
        private static CashTransactionResponse CreateCashTransactionResponse(CashTransaction cashTransaction, string iban, string sender, string recipient)
        {
            var isTransferFees = cashTransaction.Type == CashTransactionType.CommissionFees;

            return new CashTransactionResponse(cashTransaction.From,
                                               cashTransaction.To,
                                               cashTransaction.From != iban ? cashTransaction.Amount : -cashTransaction.Amount,
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
        private static LastCashTransactionResponse CreateLastCashTransactionResponse(string toAccount, string recipient, decimal amount, DateTime createdOn)
        {
            return new LastCashTransactionResponse(toAccount, recipient, amount, createdOn);
        }



        [NonAction]
        private static bool IsTransferTransaction(CashTransaction cashTransaction)
        {
            return cashTransaction.Type == CashTransactionType.Transfer || cashTransaction.Type == CashTransactionType.EFT;
        }

     
        #endregion
    }
}
