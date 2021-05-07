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
        private readonly ICustomerService _customerService;
        private readonly IBankAccountRepository _bankAccountRepo;
        private readonly ICashTransactionsRepository _cashTransactionsRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashTransactionsService(VirtualBankDbContext dbContext,
                                       ICustomerService customersService,
                                       IBankAccountRepository bankAccountRepo,
                                       ICashTransactionsRepository cashTransactionsRepo,
                                       IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _customerService = customersService;
            _bankAccountRepo = bankAccountRepo;
            _cashTransactionsRepo = cashTransactionsRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// retrieve all transactions that occured in the specified account(from or to)
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionListResponse>> GetCashTransactionsByIBANAsync(string iban, int lastDays,
                                                                                                   int pageNumber, int pageSize,
                                                                                                   CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionListResponse>();
            var skip = (pageNumber - 1) * pageSize;

            var ibanCashTransactions = await _cashTransactionsRepo.GetByIBAN(iban, lastDays);

            if(ibanCashTransactions.Count() == 0) {
                return responseModel;
            }

            var cashTransactions = ibanCashTransactions.OrderByDescending(c => c.CreatedAt).Skip(skip).Take(pageSize);

            var cashTransactionList = new List<CashTransactionResponse>();

            foreach (var cashTransaction in cashTransactions)
            {
                if (cashTransaction.From != iban && IsTransferTransaction(cashTransaction))
                {
                    var senderResponse = await _customerService.GetCustomerByIBANAsync(cashTransaction.From, cancellationToken);
                    var sender = senderResponse?.Data?.FullName;

                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, sender, iban, Direction.In));
                }

                else if(cashTransaction.To != iban && IsTransferTransaction(cashTransaction))
                {
                    var recipientResponse = await _customerService.GetCustomerByIBANAsync(cashTransaction.To, cancellationToken);
                    var recipient = recipientResponse?.Data?.FullName;

                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, recipient, Direction.Out));
                }

                else if (cashTransaction.Type == CashTransactionType.Deposit)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, "", cashTransaction.To, Direction.In));
                }
                else if (cashTransaction.Type == CashTransactionType.Withdrawal)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, cashTransaction.From, "", Direction.Out));
                }
                else if (cashTransaction.Type == CashTransactionType.CommissionFees)
                {
                    cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, cashTransaction.From, "", Direction.Out));
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


            responseModel.Data = CreateCashTransactionResponse(lastTransaction, lastTransaction.From, lastTransaction.To, Direction.In);

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

            var user = _httpContextAccessor.HttpContext.User;
            var username = user.Identity.Name;
            var amountToDeposit = request.CashTransaction.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var toAccount = await _bankAccountRepo.FindByIBANAsync(request.CashTransaction.To);

                if (toAccount == null)
                {
                    responseModel.AddError("account not found");
                    return responseModel;
                }

                //Add amount to recipient balance
                toAccount.Balance += amountToDeposit;


                await _bankAccountRepo.UpdateAsync(_dbContext, toAccount);

                var cashTransaction = CreateCashTransaction(request, username, 0, toAccount.Balance);

                //Add transaction to db & save changes 
                await _cashTransactionsRepo.AddAsync(_dbContext, cashTransaction);

                await dbContextTransaction.CommitAsync();

                return responseModel;
            }

            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync();
                responseModel.AddError(ex.ToString());

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

            var user = _httpContextAccessor.HttpContext.User;
            var username = user.Identity.Name;
            var amountToWithdraw = request.CashTransaction.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var fromAccount = await _bankAccountRepo.FindByIBANAsync(request.CashTransaction.From);
                bool isBlocked = true;

                if (fromAccount == null)
                {
                    responseModel.AddError("account not found");
                    return responseModel;
                }

                if (fromAccount.Type == AccountType.Savings)
                {
                    var deposits = await _cashTransactionsRepo.GetDepositsByIBAN(fromAccount.IBAN);

                    foreach (var deposit in deposits)
                    {
                        if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 180)
                        {
                            isBlocked = false;
                        }
                    }

                    if (isBlocked)
                    {
                        responseModel.AddError("Deposit is blocked");
                        return responseModel;
                    }
                }

                if (amountToWithdraw <= fromAccount.AllowedBalanceToUse)
                {
                    fromAccount.Balance -= amountToWithdraw;

                    await _bankAccountRepo.UpdateAsync(_dbContext, fromAccount);

                    var cashTransaction = CreateCashTransaction(request, username, fromAccount.Balance, 0);

                    await _cashTransactionsRepo.AddAsync(_dbContext, cashTransaction);

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

            var user = _httpContextAccessor.HttpContext.User;
            var username = user.Identity.Name;
            var amountToTransfer = request.CashTransaction.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var senderAccount = await _bankAccountRepo.FindByIBANAsync(request.CashTransaction.From);
                var recipientAccount = await _bankAccountRepo.FindByIBANAsync(request.CashTransaction.To);

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


                if (amountToTransfer <= senderAccount.AllowedBalanceToUse)
                {
                    //Deduct from sender account
                    senderAccount.Balance -= amountToTransfer;

                    await _bankAccountRepo.UpdateAsync(_dbContext, senderAccount);

                    //Deposit to recipient account
                    recipientAccount.Balance += amountToTransfer;

                    await _bankAccountRepo.UpdateAsync(recipientAccount);

                    //Create a transaction 
                    var cashTransaction = CreateCashTransaction(request, username, senderAccount.Balance, recipientAccount.Balance);

                    //Save transaction into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, cashTransaction);

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

            var user = _httpContextAccessor.HttpContext.User;
            var username = user.Identity.Name;
            var amountToTransfer = request.CashTransaction.Amount;

            var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var senderAccount = await _bankAccountRepo.FindByIBANAsync(request.CashTransaction.From);
                var recipientAccount = await _bankAccountRepo.FindByIBANAsync(request.CashTransaction.To);

                if (senderAccount == null)
                {
                    responseModel.AddError("sender account not found");
                    return responseModel;
                }

                if (recipientAccount == null)
                {
                    responseModel.AddError("recipient account not found");
                    return responseModel;
                }

                if (recipientAccount.Owner.FirstName != request.RecipeintFirstName || recipientAccount.Owner.LastName != request.RecipeintLastName)
                {
                    responseModel.AddError("Name is not valid");
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

                    //Create a transaction 
                    var cashTransaction = CreateCashTransaction(request, username, senderAccount.Balance, recipientAccount.Balance);

                    //Save transaction into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, cashTransaction);

                    //Deduct commission fees from sender account
                    senderAccount.Balance -= (decimal)fees;

                    //Update entity & Save it to db
                    await _bankAccountRepo.UpdateAsync(_dbContext, senderAccount);

                    //Modify request for commission fees transaction
                    request.CashTransaction.Type = CashTransactionType.CommissionFees;
                    request.CashTransaction.Amount = (decimal)fees;

                    //Create a transaction for the comission fees
                    var commissionFeeTransaction = CreateCashTransaction(request, username, senderAccount.Balance, 0);

                    //Save transaction into db
                    await _cashTransactionsRepo.AddAsync(_dbContext, commissionFeeTransaction);

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
            Status = cashTransaction.TransactionDate == DateTime.Today ? TransactionStatusType.Succeeded : TransactionStatusType.Pending
          };

        }

        [NonAction]
        private CashTransactionResponse CreateCashTransactionResponse(CashTransaction cashTransaction, string sender,
                                                                     string recipient, Direction direction)                        
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
