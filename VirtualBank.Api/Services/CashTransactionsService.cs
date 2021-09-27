using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Models;
using VirtualBank.Core.Models.Responses;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CashTransactionsService : ICashTransactionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICashTransactionsMapper _cashTransactionsMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CashTransactionsService(IUnitOfWork unitOfWork,
                                       ICashTransactionsMapper cashTransactionsMapper,
                                       IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _cashTransactionsMapper = cashTransactionsMapper;
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

            var cashTransactionList = new List<CashTransaction>();

            foreach (var cashTransaction in cashTransactions)
            {
                var sender = await GetCustomerName(cashTransaction.From);
                var recipient = await GetCustomerName(cashTransaction.To);

                cashTransactionList.Add(_cashTransactionsMapper.MapToResponseModel(cashTransaction, cashTransaction.From, sender, recipient));
            }

            responseModel.Data = new(cashTransactionList.ToImmutableList(), cashTransactionList.Count);

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

            var cashTransactionList = new List<CashTransaction>();

            foreach (var cashTransaction in cashTransactions)
            {
                if (cashTransaction.From != iban && IsTransferTransaction(cashTransaction))
                {
                    var sender = await GetCustomerName(cashTransaction.From);

                    cashTransactionList.Add(_cashTransactionsMapper.MapToResponseModel(cashTransaction, iban, sender, accountHolderName));
                }

                else if (cashTransaction.To != iban && IsTransferTransaction(cashTransaction))
                {
                    var recipient = await GetCustomerName(cashTransaction.To);

                    cashTransactionList.Add(_cashTransactionsMapper.MapToResponseModel(cashTransaction, iban, accountHolderName, recipient));
                }

                else
                    switch (cashTransaction.Type)
                    {
                        case CashTransactionType.Deposit:
                            cashTransactionList.Add(_cashTransactionsMapper.MapToResponseModel(cashTransaction,
                                                                                               iban, Enum.GetName(typeof(BankAssetType),
                                                                                               cashTransaction.InitiatedBy),
                                                                                               accountHolderName));
                            break;
                        case CashTransactionType.Withdrawal:
                            cashTransactionList.Add(_cashTransactionsMapper.MapToResponseModel(cashTransaction,
                                                                                               iban, accountHolderName,
                                                                                               Enum.GetName(typeof(BankAssetType),
                                                                                               cashTransaction.InitiatedBy)));
                            break;
                    }
            }

            responseModel.Data = new(cashTransactionList.ToImmutableList(), cashTransactionList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrive last cash transactions of the specified iban
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<LatestTransferListResponse>> GetBankAccountLatestTransfersAsync(string iban,
            CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LatestTransferListResponse>();

            var latestTransfers = await _unitOfWork.CashTransactions.GetLastByIBANAsync(iban);

            if (!latestTransfers.Any())
            {
                return responseModel;
            }

            var latestTransfersList = latestTransfers.OrderByDescending(c => c.CreatedOn)
                                                     .Where(c => IsTransferTransaction(c)).Take(7)
                                                     .Select(tx => _cashTransactionsMapper.MapToLatestTransferResponseModel(tx.To, GetCustomerName(tx.To).Result,
                                                                                                                            new Amount(tx.Amount), tx.TransactionDate))
                                                     .ToImmutableList();


            responseModel.Data = new(latestTransfersList, latestTransfersList.Count);

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

            responseModel.Data = new(_cashTransactionsMapper.MapToResponseModel(lastTransaction, iban, "", ""));

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


            var toAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.To);

            if (toAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(toAccount)));
                return responseModel;
            }

            using (var dbContextTransaction = await _unitOfWork.CreateTransactionAsync())
            {
                try
                {
                    //Add amount to recipient balance
                    toAccount.Balance.Add(amountToDeposit);
                    toAccount.AllowedBalanceToUse.Add(amountToDeposit);

                    //Update bank account
                    await _unitOfWork.BankAccounts.UpdateAsync(toAccount);
                    await _unitOfWork.SaveAsync();

                    //Add transaction to db & save changes 
                    var createdCashTransaction = await _unitOfWork.CashTransactions.AddAsync(CreateCashTransaction(request, 0, toAccount.Balance));
                    await _unitOfWork.SaveAsync();

                    await dbContextTransaction.CommitAsync();

                    var depositor = await GetCustomerName(request.To);
                    var initiatedBy = GetInitiatedBy(request.InitiatedBy);

                    responseModel.Data = new(_cashTransactionsMapper.MapToResponseModel(createdCashTransaction, request.To, initiatedBy, depositor));

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


            if (amountToWithdraw > fromAccount.AllowedBalanceToUse)

            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(fromAccount), "balance is not enough to complete withdrawal"));
                return responseModel;
            }

            using (var dbContextTransaction = await _unitOfWork.CreateTransactionAsync())
            {
                try
                {
                    fromAccount.Balance.Subtract(amountToWithdraw);
                    fromAccount.AllowedBalanceToUse.Subtract(amountToWithdraw);

                    await _unitOfWork.BankAccounts.UpdateAsync(fromAccount);
                    await _unitOfWork.SaveAsync();

                    var createdCashTransaction = await _unitOfWork.CashTransactions.AddAsync(CreateCashTransaction(request, fromAccount.Balance, 0));
                    await _unitOfWork.SaveAsync();

                    await dbContextTransaction.CommitAsync();

                    var withdrawer = await GetCustomerName(request.From);
                    var initiatedBy = GetInitiatedBy(request.InitiatedBy);

                    responseModel.Data = new(_cashTransactionsMapper.MapToResponseModel(createdCashTransaction, request.From, withdrawer, initiatedBy));

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
            var recipientName = request.RecipientFirstName + ' ' + request.RecipientLastName;

            var senderBankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.From);

            if (senderBankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(senderBankAccount), "sender's bank account not found"));
                return responseModel;
            }

            var recipientBankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.To);

            if (recipientBankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(recipientBankAccount), "recipient's account not found"));
                return responseModel;
            }

            var result = ValidateTransfer(senderBankAccount, recipientBankAccount, currency, recipientName);

            if (!result.Success)
                return result;

            using (var dbContextTransaction = await _unitOfWork.CreateTransactionAsync())
            {
                try
                {
                    //Update sender bank account
                    senderBankAccount = UpdateSenderBankAccount(senderBankAccount, amountToTransfer);
                    await _unitOfWork.BankAccounts.UpdateAsync(senderBankAccount);
                    await _unitOfWork.SaveAsync();

                    //Update recipient bank account
                    recipientBankAccount = UpdateRecipientBankAccount(recipientBankAccount, amountToTransfer);
                    await _unitOfWork.BankAccounts.UpdateAsync(recipientBankAccount);
                    await _unitOfWork.SaveAsync();

                    //Create & Save transaction into db
                    var createdCashTransaction = await _unitOfWork.CashTransactions.AddAsync(CreateCashTransaction(request, senderBankAccount.Balance, recipientBankAccount.Balance));
                    await _unitOfWork.SaveAsync();

                    await dbContextTransaction.CommitAsync();

                    var sender = await GetCustomerName(request.From);
                    var recipient = await GetCustomerName(request.To);

                    responseModel.Data = new(_cashTransactionsMapper.MapToResponseModel(createdCashTransaction, request.From, sender, recipient));

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
            var recipientName = request.RecipientFirstName + ' ' + request.RecipientLastName;

            var senderBankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.From);

            if (senderBankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(senderBankAccount), "sender's bank account not found"));
                return responseModel;
            }

            var recipientBankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.To);

            if (recipientBankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(recipientBankAccount), "Recipient's bank account not found"));
                return responseModel;
            }

            var result = ValidateTransfer(senderBankAccount, recipientBankAccount, currency, recipientName);

            if (!result.Success)
                return result;

            using (var dbContextTransaction = await _unitOfWork.CreateTransactionAsync())
            {
                try
                {
                    var fees = GetFees(amountToTransfer);
                    senderBankAccount = UpdateSenderBankAccount(senderBankAccount, amountToTransfer, true);
                    await _unitOfWork.BankAccounts.UpdateAsync(senderBankAccount);
                    await _unitOfWork.SaveAsync();

                    //Update recipient bank account
                    recipientBankAccount = UpdateRecipientBankAccount(recipientBankAccount, amountToTransfer);
                    await _unitOfWork.BankAccounts.UpdateAsync(recipientBankAccount);
                    await _unitOfWork.SaveAsync();

                    //Create & Save transaction into db
                    var createdCashTransaction = await _unitOfWork.CashTransactions.AddAsync(CreateCashTransaction(request, senderBankAccount.Balance, recipientBankAccount.Balance, fees));
                    await _unitOfWork.SaveAsync();

                    await dbContextTransaction.CommitAsync();

                    var sender = await GetCustomerName(request.From);
                    var recipient = await GetCustomerName(request.To);
                    var feesWithCurrency = new Money(fees, currency);

                    responseModel.Data = new(_cashTransactionsMapper.MapToResponseModel(createdCashTransaction, request.From, sender, recipient, feesWithCurrency));

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
        private Core.Entities.CashTransaction CreateCashTransaction(CreateCashTransactionRequest request, decimal senderBalance,
                                                                    decimal recipientBalance, decimal fees = 0) =>
            new()
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

        private ApiResponse<CashTransactionResponse> ValidateTransfer(Core.Entities.BankAccount senderBankAccount,
                                                                      Core.Entities.BankAccount recipientBankAccount,
                                                                      string currency,
                                                                      string recipientName)
        {
            var responseModel = new ApiResponse<CashTransactionResponse>();

            if (senderBankAccount.IBAN == recipientBankAccount.IBAN)
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError("sender should send to a different bad account"));
                return responseModel;
            }

            if (recipientBankAccount.Owner.FirstName + ' ' + recipientBankAccount.Owner.LastName != recipientName)
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(recipientBankAccount), "recipient name does not match account holder's name"));
                return responseModel;
            }

            if (senderBankAccount.Type != AccountType.Current || senderBankAccount.Type != AccountType.Recurring)
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(senderBankAccount), $"transaction is not allowed, {Enum.GetName(typeof(AccountType), senderBankAccount.Type)} account type "));
                return responseModel;
            }

            if (currency != senderBankAccount.Currency.Code)
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError(nameof(currency), $"Funds currency should match the correny of the bank account"));
                return responseModel;
            }

            return responseModel;
        }

        private static Core.Entities.BankAccount UpdateSenderBankAccount(Core.Entities.BankAccount senderBankAccount, Amount amountToTransfer, bool isEFT = false)
        {
            var fees = isEFT ? GetFees(amountToTransfer) : new Amount(0);
            var debt = new Amount(0);
            var amountToSubtract = new Amount(amountToTransfer + fees);

            if (senderBankAccount.AllowedBalanceToUse - amountToSubtract < 0)
            {
                debt = amountToSubtract.Subtract(senderBankAccount.AllowedBalanceToUse);
                amountToSubtract = senderBankAccount.AllowedBalanceToUse;
            }

            //Deduct from sender account
            senderBankAccount.Balance.Subtract(amountToSubtract);
            senderBankAccount.AllowedBalanceToUse.Subtract(amountToSubtract);

            if (debt > 0)
                senderBankAccount.Debt.Add(debt);

            return senderBankAccount;
        }

        private static Core.Entities.BankAccount UpdateRecipientBankAccount(Core.Entities.BankAccount recipientBankAccount, Amount amountToTransfer)
        {
            var recipientDebt = recipientBankAccount.Debt;
            var amountToAdd = new Amount(amountToTransfer - recipientDebt);

            if (amountToTransfer > recipientDebt)
            {
                recipientBankAccount.Debt.Subtract(recipientDebt);
                recipientBankAccount.Balance.Add(amountToAdd);
                recipientBankAccount.AllowedBalanceToUse.Add(amountToAdd);
            }
            else
                recipientBankAccount.Debt.Subtract(amountToTransfer);

            return recipientBankAccount;
        }

        private static Amount GetFees(Amount amountToTransfer)
        {
            const decimal feesRate = (decimal)0.0015;
            return new Amount(amountToTransfer.Value * feesRate);
        }


        private async Task<string> GetCustomerName(string iban)
        {
            var customer = await _unitOfWork.Customers.FindByIBANAsync(iban);
            return customer != null ? customer.FirstName + " " + customer.LastName : string.Empty;
        }

        private static bool IsTransferTransaction(Core.Entities.CashTransaction cashTransaction) =>
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