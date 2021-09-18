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
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CashTransactionsService : ICashTransactionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashTransactionsService(IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
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
        public async Task<ApiResponse<CashTransactionListResponse>> GetAllCashTransactionsAsync(int pageNumber, int pageSize,
                                                                                                CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionListResponse>();

            var allCashTransactions = await _unitOfWork.CashTransactions.GetAllAsync();

            if (!allCashTransactions.Any())
            {
                return responseModel;
            }

            var cashTransactions = allCashTransactions.OrderByDescending(c => c.CreatedOn)
                .Skip((pageNumber - 1) * pageSize)
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

            var accountHolder = await _unitOfWork.Customers.FindByIBANAsync(iban);
            var accountHolderName = accountHolder.FirstName + " " + accountHolder.LastName;


            var accountCashTransactions = await _unitOfWork.CashTransactions.GetByIBANAsync(iban, lastDays);

            if (!accountCashTransactions.Any())
            {
                return responseModel;
            }

            var cashTransactions = accountCashTransactions.OrderByDescending(c => c.CreatedOn)
                                                          .Skip((pageNumber - 1) * pageSize)
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

                else
                    switch (cashTransaction.Type)
                    {
                        case CashTransactionType.Deposit:
                            cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, Enum.GetName(typeof(BankAssetType), cashTransaction.InitiatedBy), accountHolderName));
                            break;
                        case CashTransactionType.Withdrawal:
                            cashTransactionList.Add(CreateCashTransactionResponse(cashTransaction, iban, accountHolderName, Enum.GetName(typeof(BankAssetType), cashTransaction.InitiatedBy)));
                            break;
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
        public async Task<ApiResponse<LastCashTransactionListResponse>> GetLatestTransfersAsync(string iban,
            CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LastCashTransactionListResponse>();

            var latestTransfers = await _unitOfWork.CashTransactions.GetLastByIBANAsync(iban);

            if (!latestTransfers.Any())
            {
                return responseModel;
            }

            var latestTransfersList = latestTransfers.OrderByDescending(c => c.CreatedOn)
                .Where(c => IsTransferTransaction(c)).Take(7)
                .Select(async tx => CreateLatestTransferResponse(tx.To, await GetCustomerName(tx.To),
                    new Amount(tx.Amount), tx.TransactionDate))
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
        public async Task<ApiResponse<CashTransactionResponse>> GetLastCashTransactionAsync(string iban,
            CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();

            var lastTransaction = await _unitOfWork.CashTransactions.FindByIBANAsync(iban);

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
        public async Task<ApiResponse<CashTransactionResponse>> MakeDepositAsync(CreateCashTransactionRequest request,
            CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToDeposit = request.DebitedFunds.Amount;
            var currency = request.DebitedFunds.Currency;

            try
            {
                var toAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.To);

                if (toAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(toAccount)));
                    return responseModel;
                }

                //Add amount to recipient balance
                toAccount.Balance.Add(amountToDeposit);
                toAccount.AllowedBalanceToUse.Add(amountToDeposit);

                //Update bank account
                await _unitOfWork.BankAccounts.UpdateAsync(toAccount);

                //Add transaction to db & save changes 
                var createdCashTransaction = await _unitOfWork.CashTransactions.AddAsync(CreateCashTransaction(request, 0, toAccount.Balance));

                var depositor = await GetCustomerName(request.To);
                var initiatedBy = GetInitiatedBy(request.InitiatedBy);

                responseModel.Data = CreateCashTransactionResponse(createdCashTransaction, request.To, initiatedBy, depositor);

                await _unitOfWork.CompleteTransactionAsync();

                return responseModel;
            }

            catch (Exception ex)
            {
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
        public async Task<ApiResponse<CashTransactionResponse>> MakeWithdrawalAsync(
            CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToWithdraw = request.DebitedFunds.Amount;
            var currency = request.DebitedFunds.Currency;

            try
            {
                var fromAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.From);
                bool isBlocked = true;

                if (fromAccount == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(fromAccount)));
                    return responseModel;
                }

                if (fromAccount.Type == AccountType.Savings)
                {
                    var deposits = await _unitOfWork.CashTransactions.GetDepositsByIBANAsync(fromAccount.IBAN);

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

                    await _unitOfWork.BankAccounts.UpdateAsync(fromAccount);

                    var createdCashTransaction = await _unitOfWork.CashTransactions.AddAsync(CreateCashTransaction(request, fromAccount.Balance, 0));

                    var withDrawer = await GetCustomerName(request.From);
                    var initiatedBy = GetInitiatedBy(request.InitiatedBy);

                    responseModel.Data = CreateCashTransactionResponse(createdCashTransaction, request.From, withDrawer, initiatedBy);

                    await _unitOfWork.CompleteTransactionAsync();

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
        public async Task<ApiResponse<CashTransactionResponse>> MakeTransferAsync(CreateCashTransactionRequest request,
            CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToTransfer = request.DebitedFunds.Amount;
            var currency = request.DebitedFunds.Currency;


            var senderAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.From);
            var recipientAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.To);

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

            using (var dbContextTransaction = await _unitOfWork.CreateTransactionAsync())
            {
                try
                {
                    var debt = new Amount(0);
                    var amountToSubtract = new Amount(amountToTransfer);

                    if (senderAccount.AllowedBalanceToUse - amountToSubtract < 0)
                    {
                        debt = amountToSubtract.Subtract(senderAccount.AllowedBalanceToUse);
                        amountToSubtract = senderAccount.AllowedBalanceToUse;
                    }

                    //Deduct from sender account
                    senderAccount.Balance.Subtract(amountToSubtract);
                    senderAccount.AllowedBalanceToUse.Subtract(amountToSubtract);

                    if (debt > 0)
                        senderAccount.Debt.Add(debt);

                    //Update sender bank account
                    await _unitOfWork.BankAccounts.UpdateAsync(senderAccount);
                    await _unitOfWork.SaveAsync();

                    //Deposit to recipient account
                    recipientAccount.Balance.Add(amountToTransfer);
                    senderAccount.AllowedBalanceToUse.Add(amountToTransfer);

                    //Update recipient bank account
                    await _unitOfWork.BankAccounts.UpdateAsync(recipientAccount);
                    await _unitOfWork.SaveAsync();

                    //Create & Save transaction into db
                    var createdCashTransaction = await _unitOfWork.CashTransactions.AddAsync(CreateCashTransaction(request, senderAccount.Balance, recipientAccount.Balance));
                    await _unitOfWork.SaveAsync();

                    var sender = await GetCustomerName(request.From);
                    var recipient = await GetCustomerName(request.To);

                    responseModel.Data = CreateCashTransactionResponse(createdCashTransaction, request.From, sender, recipient);

                    await dbContextTransaction.CommitAsync();

                    return responseModel;
                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                    return responseModel;
                }
            }
        }


        /// <summary>
        /// Create an EFT Transfer transaction in db (from/to accounts in different banks)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CashTransactionResponse>> MakeEFTTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();
            var amountToTransfer = request.DebitedFunds.Amount;
            var currency = request.DebitedFunds.Currency;

            var senderAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.From);

            if (senderAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(senderAccount), "sender account not found"));
                return responseModel;
            }

            var recipientAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.To);

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

            if (recipientAccount.Owner.FirstName != request.RecipientFirstName ||
                recipientAccount.Owner.LastName != request.RecipientLastName)
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

            using (var dbContextTransaction = await _unitOfWork.CreateTransactionAsync())
            {
                try
                {
                    const decimal feesRate = (decimal)0.0015;
                    var fees = new Amount(amountToTransfer.Value * feesRate);
                    var debt = new Amount(0);
                    var amountToSubtract = new Amount(amountToTransfer + fees);

                    if (senderAccount.AllowedBalanceToUse - amountToSubtract < 0)
                    {
                        debt = amountToSubtract.Subtract(senderAccount.AllowedBalanceToUse);
                        amountToSubtract = senderAccount.AllowedBalanceToUse;
                    }

                    //Deduct from sender account
                    senderAccount.Balance.Subtract(amountToSubtract);
                    senderAccount.AllowedBalanceToUse.Subtract(amountToSubtract);

                    if (debt > 0)
                        senderAccount.Debt.Add(debt);

                    await _unitOfWork.BankAccounts.UpdateAsync(senderAccount);
                    await _unitOfWork.SaveAsync();

                    //Deposit to recipient account
                    recipientAccount.Balance.Add(amountToTransfer);
                    recipientAccount.AllowedBalanceToUse.Add(amountToTransfer);

                    await _unitOfWork.BankAccounts.UpdateAsync(recipientAccount);
                    await _unitOfWork.SaveAsync();

                    //Create & Save transaction into db
                    var createdCashTransaction = await _unitOfWork.CashTransactions.AddAsync(CreateCashTransaction(request, senderAccount.Balance, recipientAccount.Balance, fees));
                    await _unitOfWork.SaveAsync();

                    var sender = await GetCustomerName(request.From);
                    var recipient = await GetCustomerName(request.To);

                    responseModel.Data = CreateCashTransactionResponse(createdCashTransaction, request.From, sender, recipient, CreateMoney(fees, currency));

                    await dbContextTransaction.CommitAsync();

                    return responseModel;

                }
                catch (Exception ex)
                {
                    await dbContextTransaction.RollbackAsync();
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                    return responseModel;
                }
            }
        }


        #region private Helper methods

        [NonAction]
        private CashTransaction CreateCashTransaction(CreateCashTransactionRequest request, decimal senderBalance, decimal recipientBalance, decimal fees = 0)
        {
            return new CashTransaction()
            {
                Type = request.Type,
                From = request.From,
                To = request.To,
                Amount = request.DebitedFunds.Amount,
                Fees = fees > 0 ? fees : 0,
                Currency = request.DebitedFunds.Currency,
                SenderRemainingBalance = senderBalance,
                RecipientRemainingBalance = recipientBalance,
                InitiatedBy = request.InitiatedBy,
                Description = request.Description,
                PaymentType = request.PaymentType,
                TransactionDate = request.TransactionDate,
                CreditCardNo = request.CreditCardNo,
                DebitCardNo = request.DebitCardNo,
                CreatedOn = request.CreationInfo.CreatedOn,
                CreatedBy = request.CreationInfo.CreatedBy
            };
        }

        [NonAction]
        private static CashTransactionResponse CreateCashTransactionResponse(CashTransaction cashTransaction, string iban, string sender, string recipient, Money fees = null)
        {
            return new CashTransactionResponse(cashTransaction.From,
                cashTransaction.To,
                sender,
                recipient,
                cashTransaction.From != iban
                    ? CreateDebitedFunds(cashTransaction.Amount, cashTransaction.Currency)
                    : CreateDebitedFunds(-cashTransaction.Amount, cashTransaction.Currency),
                fees ?? CreateMoney(new Amount(0), ""),
                cashTransaction.PaymentType,
                cashTransaction.From != iban ? $"From: {sender}, Account No: {cashTransaction.From} "
                : cashTransaction.InitiatedBy == BankAssetType.POS ? cashTransaction.DebitCardNo != null
                    ? $"{cashTransaction.InitiatedBy} purchase: card No: {cashTransaction.DebitCardNo}, {recipient}"
                    : $"{cashTransaction.InitiatedBy} purchase: card No: {cashTransaction.CreditCardNo}, {recipient}"
                : $"{cashTransaction.To}--{recipient}, {cashTransaction.Description}",
                cashTransaction.InitiatedBy,
                cashTransaction.From != iban
                    ? CreateMoney(new Amount(cashTransaction.RecipientRemainingBalance), cashTransaction.Currency)
                    : CreateMoney(new Amount(cashTransaction.SenderRemainingBalance), cashTransaction.Currency),
                CreateCreationInfo(cashTransaction.CreatedBy, cashTransaction.CreatedOn));
        }


        [NonAction]
        private static LastCashTransactionResponse CreateLatestTransferResponse(string toAccount, string recipient,
            Amount amount, DateTime createdOn)
        {
            return new LastCashTransactionResponse(toAccount, recipient, amount, createdOn);
        }

        [NonAction]
        private static Money CreateDebitedFunds(decimal amount, string currency) => new Money(new Amount(amount), currency);

        [NonAction]
        private static Money CreateMoney(Amount amount, string currency) => new Money(amount, currency);

        [NonAction]
        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new CreationInfo(createdBy, createdOn);

        [NonAction]
        private async Task<string> GetCustomerName(string iban)
        {
            var customer = await _unitOfWork.Customers.FindByIBANAsync(iban);
            var customerFullName = customer.FirstName + " " + customer.LastName;

            return customerFullName;
        }


        [NonAction]
        private static bool IsTransferTransaction(CashTransaction cashTransaction) =>
                cashTransaction.Type == CashTransactionType.Transfer ||
                cashTransaction.Type == CashTransactionType.EFT;


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