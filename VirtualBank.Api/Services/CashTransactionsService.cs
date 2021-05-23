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

            var ibanCashTransactions = await _cashTransactionsRepo.GetAllAsync();

            if (ibanCashTransactions.Count() == 0)
            {
                return responseModel;
            }

            var cashTransactions = ibanCashTransactions.OrderByDescending(c => c.CreatedAt).Skip((pageNumber - 1) * pageSize)
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionListResponse>> GetCashTransactionsByIBANAsync(string iban, int lastDays,
                                                                                                   int pageNumber, int pageSize,
                                                                                                   CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionListResponse>();

            var accountHolder = await _customerRepo.FindByIBANAsync(iban);
            var accountHolderlName =  accountHolder.FirstName + " " + accountHolder.LastName;


            var accountCashTransactions = await _cashTransactionsRepo.GetByIBANAsync(iban, lastDays);

            if (accountCashTransactions.Count() == 0)
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

                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, senderFullName, accountHolderlName));
                }

                else if (cashTransaction.To != iban && IsTransferTransaction(cashTransaction))
                {
                    var recipient = await _customerRepo.FindByIBANAsync(cashTransaction.To);
                    var recipientFullName = recipient.FirstName + " " + recipient.LastName;

                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderlName, recipientFullName));
                }

                else if (cashTransaction.Type == CashTransactionType.Deposit)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, "ATM", accountHolderlName));
                }
                else if (cashTransaction.Type == CashTransactionType.Withdrawal)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderlName, "ATM"));
                }
                else if (cashTransaction.Type == CashTransactionType.CommissionFees)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderlName, "commission fees"));
                }
            }

            responseModel.Data = new CashTransactionListResponse(cashTransactionList.ToImmutableList(), cashTransactionList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve last transaction that occured the specified account(from or to)
        /// </summary>
        /// <param name="accountNo"></param>
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
        public async Task<ApiResponse> MakeDepositAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();

            var amountToDeposit = request.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

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

                //Update bank account
                await _bankAccountRepo.UpdateAsync(_dbContext, toAccount);

                //Add transaction to db & save changes 
                await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, 0, toAccount.Balance));

                await dbContextTransaction.CommitAsync();

                return responseModel;
            }

            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync();
                responseModel.AddError(ExceptionCreator.CreateInternalServerError());

                return responseModel;
            }
        }


        /// <summary>
        /// Create a withdrawal transaction in db
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> MakeWithdrawalAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();

            var amountToWithdraw = request.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

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
                responseModel.AddError(ExceptionCreator.CreateInternalServerError());

                return responseModel;
            }
        }


        /// <summary>
        /// Create a Transfer transaction in db (between acoounts in the same bank)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> MakeTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();

            var amountToTransfer = request.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

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

                    //Update sender bank account
                    await _bankAccountRepo.UpdateAsync(_dbContext, senderAccount);

                    //Deposit to recipient account
                    recipientAccount.Balance += amountToTransfer;

                    //Update recipient bank account
                    await _bankAccountRepo.UpdateAsync(_dbContext, recipientAccount);

                    //Create & Save transaction into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, senderAccount.Balance, recipientAccount.Balance));

                    await dbContextTransaction.CommitAsync();

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
                await dbContextTransaction.RollbackAsync();
                responseModel.AddError(ExceptionCreator.CreateInternalServerError());

                return responseModel;
            }
        }


        /// <summary>
        /// Create an EFT Transfer transaction in db (from/to acoounts in different banks)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> MakeEFTTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToTransfer = request.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

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

                    await _bankAccountRepo.UpdateAsync(_dbContext, senderAccount);

                    //Deposit to recipient account
                    recipientAccount.Balance += amountToTransfer;

                    await _bankAccountRepo.UpdateAsync(_dbContext, recipientAccount);


                    //Create & Save transaction into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, senderAccount.Balance, recipientAccount.Balance));

                    //Deduct commission fees from sender account
                    senderAccount.Balance -= (decimal)fees;

                    //Update entity & Save it to db
                    await _bankAccountRepo.UpdateAsync(_dbContext, senderAccount);

                    //Modify request for commission fees transaction
                    request.Type = CashTransactionType.CommissionFees;
                    request.Amount = (decimal)fees;

                    //Create & Save commission fees into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, CreateCashTransaction(request, senderAccount.Balance, 0));

                    await dbContextTransaction.CommitAsync();

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
                await dbContextTransaction.RollbackAsync();
                responseModel.AddError(ExceptionCreator.CreateInternalServerError());

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
            };

        }

        [NonAction]
        private CashTransactionResponse CreateCashTransactionResponse(CashTransaction cashTransaction, string iban, string sender, string recipient)
        {
            var isTransferFees = cashTransaction.Type == CashTransactionType.CommissionFees;

            return new CashTransactionResponse(cashTransaction.From,
                                               cashTransaction.To,
                                               cashTransaction.From != iban ? cashTransaction.Amount : -cashTransaction.Amount,
                                               sender,
                                               recipient,
                                               cashTransaction.PaymentType,
                                               isTransferFees ? "Transafer comission Fees" :
                                                cashTransaction.From != iban ?
                                                $"From: {sender}, Account No: {cashTransaction.From} "
                                                :
                                                cashTransaction.InitiatedBy == BankAssetType.POS || cashTransaction.InitiatedBy == BankAssetType.ATM ?
                                                $"{cashTransaction.InitiatedBy} purchase: {recipient}"
                                                :
                                                $"{cashTransaction.To}--{recipient}, {cashTransaction.Description}",
                                               cashTransaction.InitiatedBy,
                                               cashTransaction.From != iban ? cashTransaction.RecipientRemainingBalance : cashTransaction.SenderRemainingBalance,
                                               cashTransaction.CreatedAt,
                                               cashTransaction.CreatedBy);
        }


        private bool IsTransferTransaction(CashTransaction cashTransaction)
        {
            return cashTransaction.Type == CashTransactionType.Transfer || cashTransaction.Type == CashTransactionType.EFT;
        }

        #endregion
    }
}
