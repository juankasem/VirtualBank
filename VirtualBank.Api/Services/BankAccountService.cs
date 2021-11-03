﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Helpers.Methods;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.BankAccountApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.BankAccountApiResponses;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Models;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBankAccountMapper _bankAccountMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BankAccountService(IUnitOfWork unitOfWork,
                                  IBankAccountMapper bankAccountMapper,
                                  IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _bankAccountMapper = bankAccountMapper;
        }

        #region public server methods
        /// <summary>
        /// Retrieve bank accounts for the specified customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BankAccountListResponse>> GetBankAccountsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountListResponse>();

            var bankAccounts = await _unitOfWork.BankAccounts.GetByCustomerId(customerId);

            if (!bankAccounts.Any())
            {
                return responseModel;
            }

            var bankAccountList = bankAccounts.OrderBy(b => b.CreationInfo.CreatedOn)
                                              .Select(bankAccount => _bankAccountMapper.MapToResponseModel(bankAccount,
                                                                                       _unitOfWork.CashTransactions.GetLastByIBANAsync(bankAccount.IBAN)
                                                                                       .GetAwaiter().GetResult().CreationInfo.CreatedOn))
                                                                                       .ToImmutableList();

            responseModel.Data = new(bankAccountList, bankAccountList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve bank account for the specified id
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BankAccountResponse>> GetBankAccountByIdAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByIdAsync(accountId);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account Id: {accountId} Not found"));
                return responseModel;
            }

            var lastTransaction = await _unitOfWork.CashTransactions.GetLastByIBANAsync(bankAccount.IBAN);

            responseModel.Data = new(_bankAccountMapper.MapToResponseModel(bankAccount, lastTransaction.CreationInfo.CreatedOn));

            return responseModel;
        }


        /// <summary>
        /// Retrieve bank account for the specified account number
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BankAccountResponse>> GetBankAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByAccountNoAsync(accountNo);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account No: {accountNo} Not found"));

                return responseModel;
            }

            var lastTransaction = await _unitOfWork.CashTransactions.GetLastByIBANAsync(bankAccount.IBAN);

            responseModel.Data = new(_bankAccountMapper.MapToResponseModel(bankAccount, lastTransaction.CreationInfo.CreatedOn));

            return responseModel;
        }


        /// <summary>
        /// Retrieve bank account for the specified iban
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BankAccountResponse>> GetBankAccountByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(iban);


            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"IBAN: {iban} Not found"));
                return responseModel;
            }

            var lastTransaction = await _unitOfWork.CashTransactions.GetLastByIBANAsync(bankAccount.IBAN);

            responseModel.Data = new(_bankAccountMapper.MapToResponseModel(bankAccount, lastTransaction.CreationInfo.CreatedOn));

            return responseModel;
        }


        /// <summary>
        /// Retrieve recipient bank account for the specified iban
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<RecipientBankAccountResponse>> ValidateRecipientBankAccountAsync(RecipientBankAccountRequest request, CancellationToken cancellationToken = default)

        {
            var responseModel = new ApiResponse<RecipientBankAccountResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.IBAN);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account of: {request.IBAN} not found"));
                return responseModel;
            }

            var bankAccountOwner = await _unitOfWork.Customers.FindByIBANAsync(bankAccount.IBAN);
            var recipientName = request.RecipientName;

            if (Utils.GetFullName(bankAccountOwner.FirstName, bankAccountOwner.LastName) != recipientName)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(recipientName), $"Recipient name: {recipientName} not found"));
                return responseModel;
            }

            responseModel.Data = new(_bankAccountMapper.MapToRecipientBankAccount(bankAccount));

            return responseModel;
        }


        /// <summary>
        /// Add or Edit bank account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BankAccountResponse>> AddOrEditBankAccountAsync(int accountId, CreateBankAccountRequest request,
                                                                                      CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            if (accountId > 0)
            {
                var bankaccount = await _unitOfWork.BankAccounts.FindByIdAsync(accountId);

                if (bankaccount != null)
                {
                    try
                    {
                        bankaccount.AccountNo = request.AccountNo;
                        bankaccount.IBAN = request.IBAN;
                        bankaccount.Owner = CreateBankAccountOwner(request.Owner.CustomerId, request.Owner.FullName, request.Owner.Gender);
                        bankaccount.Branch = CreateBankAccountBranch(request.Branch.Id, request.Branch.Code, request.Branch.Name, request.Branch.City);
                        bankaccount.Currency = CreateBankAccountCurrency(request.Currency.Id, request.Currency.Code, request.Currency.Symbol);
                        bankaccount.Balance = request.Balance;
                        bankaccount.Type = request.Type;
                        bankaccount.ModificationInfo = Utils.CreateModificationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn);

                        await _unitOfWork.BankAccounts.UpdateAsync(bankaccount);
                        await _unitOfWork.SaveAsync();
                    }
                    catch (Exception ex)
                    {
                        responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                        return responseModel;
                    }
                }
                else
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError("bankAccount", $"bank account not found"));

            }
            else
            {
                try
                {
                    await _unitOfWork.BankAccounts.AddAsync(CreateBankAccount(request));
                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                    return responseModel;
                }
            }

            return responseModel;
        }


        /// <summary>
        /// Activate bank account for for the specified id
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> ActivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByIdAsync(accountId);

            if (bankAccount != null)
            {
                bankAccount.Disabled = false;
            }
            else
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account of id: {accountId} not found"));
            }

            return responseModel;
        }

        /// <summary>
        /// Deactivate bank account for for the specified id
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> DeactivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByIdAsync(accountId);

            if (bankAccount != null)
            {
                bankAccount.Disabled = true;

            }
            else
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount)));

            return responseModel;
        }


        /// <summary>
        /// Calculate net profits of savings in savings account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BankAccountResponse>> CalculateNetProfits(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByIdAsync(accountId);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account od id: {accountId} not found"));
                return responseModel;
            }

            if (bankAccount.Type == AccountType.Savings)
            {
                var deposits = await _unitOfWork.CashTransactions.GetDepositsByIBANAsync(bankAccount.IBAN);

                foreach (var deposit in deposits)
                {
                    decimal profit = 0;
                    double interestRate = 0.00;

                    switch (bankAccount.Currency.Code)
                    {
                        case "TL":
                            try
                            {
                                if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 180 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 365)
                                {
                                    interestRate = 0.1525;
                                }
                                else if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays > 365 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 720)
                                {
                                    interestRate = 0.17;
                                }
                            }
                            catch (Exception ex)
                            {
                                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                                return responseModel;
                            }
                            break;


                        case "USD":
                            try
                            {
                                if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 180 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 365)
                                {
                                    interestRate = 0.0085;
                                }
                                else if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays > 365 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 720)
                                {
                                    interestRate = 0.01;
                                }
                            }
                            catch (Exception ex)
                            {
                                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                                return responseModel;
                            }
                            break;


                        case "EUR":

                            try
                            {
                                if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 180 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 365)
                                {
                                    interestRate = 0.035;
                                }
                                else if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays > 365 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 720)
                                {
                                    interestRate = 0.05;
                                }
                            }
                            catch (Exception ex)
                            {
                                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                                return responseModel;
                            }

                            break;

                        default:
                            return responseModel;

                    }

                    profit = deposit.DebitedFunds.Amount * (decimal)interestRate;
                    bankAccount.Balance.Add(new Amount(deposit.DebitedFunds.Amount + profit));
                }
            }

            return responseModel;
        }
        #endregion

        #region private helper methods
        private Core.Domain.Models.BankAccount CreateBankAccount(CreateBankAccountRequest request) =>
            new(0,
                request.AccountNo,
                request.IBAN,
                request.Type,
                CreateBankAccountOwner(request.Owner.CustomerId, request.Owner.FullName, request.Owner.Gender),
                CreateBankAccountBranch(request.Branch.Id, request.Branch.Code, request.Branch.Name, request.Branch.City),
                request.Balance,
                request.AllowedBalanceToUse,
                new Amount(1),
                new Amount(0),
                CreateBankAccountCurrency(request.Currency.Id, request.Currency.Code, request.Currency.Symbol),
                Utils.CreateCreationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn),
                Utils.CreateModificationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn),
                false);


        private BankAccountOwner CreateBankAccountOwner(int customerId, string name, Gender gender) =>
               new(customerId, name, gender);

        private BankAccountBranch CreateBankAccountBranch(int branchId, string code, string name, string city) =>
               new(branchId, code, name, city);

        private BankAccountCurrency CreateBankAccountCurrency(int currencyId, string code, string symbol) =>
               new(currencyId, code, symbol);

        #endregion
    }
}
