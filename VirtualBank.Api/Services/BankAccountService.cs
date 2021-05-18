using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBankAccountRepository _bankAccountRepo;
        private readonly ICashTransactionsRepository _cashTransactionsRepo;

        public BankAccountService(VirtualBankDbContext dbContext,
                                  IHttpContextAccessor httpContextAccessor,
                                  IBankAccountRepository bankAccountRepo,
                                  ICashTransactionsRepository cashTransactionsRepo)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _bankAccountRepo = bankAccountRepo;
            _cashTransactionsRepo = cashTransactionsRepo;
        }

        /// <summary>
        /// Retrieve bank accounts for the specified customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BankAccountListResponse>> GetBankAccountsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountListResponse>();

            var bankAccounts = await _bankAccountRepo.GetByCustomerId(customerId);

            if (bankAccounts.Count() == 0)
            {
                return responseModel;
            }

            var bankAccountList = new List<BankAccountResponse>();   

            foreach (var bankAccount in bankAccounts)
            {
                var accountOwner = CreateBankAccountOwner(bankAccount);
                var lastTransaction = await _cashTransactionsRepo.GetLastAsync(bankAccount.IBAN);

                bankAccountList.Add(CreateBankAccountResponse(bankAccount, accountOwner, lastTransaction.CreatedAt));
            }

            responseModel.Data = new BankAccountListResponse(bankAccountList.ToImmutableList(), bankAccountList.Count);

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

            var bankAccount = await _bankAccountRepo.FindByIdAsync(accountId);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account Id: {accountId} Not found"));

                return responseModel;
            }

            var accountOwner = CreateBankAccountOwner(bankAccount);
            var lastTransaction = await _cashTransactionsRepo.GetLastAsync(bankAccount.IBAN);

            responseModel.Data = CreateBankAccountResponse(bankAccount, accountOwner, lastTransaction.CreatedAt);

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

            var bankAccount = await _bankAccountRepo.FindByAccountNoAsync(accountNo);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account No: {accountNo} Not found"));

                return responseModel;
            }

            var accountOwner = CreateBankAccountOwner(bankAccount);
            var lastTransaction = await _cashTransactionsRepo.GetLastAsync(bankAccount.IBAN);

            responseModel.Data = CreateBankAccountResponse(bankAccount, accountOwner, lastTransaction.CreatedAt);

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

            var bankAccount = await _bankAccountRepo.FindByIBANAsync(iban);


            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"IBAN: {iban} Not found"));
                return responseModel;
            }

            var accountOwner = CreateBankAccountOwner(bankAccount);
            var lastTransaction = await _cashTransactionsRepo.GetLastAsync(bankAccount.IBAN);

            responseModel.Data = CreateBankAccountResponse(bankAccount, accountOwner, lastTransaction.CreatedAt);

            return responseModel;
        }

        /// <summary>
        /// Retrieve recipient bank account for the specified iban
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<RecipientBankAccountResponse>> GetRecipientBankAccountByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<RecipientBankAccountResponse>();

            var bankAccount = await _bankAccountRepo.FindByIBANAsync(iban);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"IBAN: {iban} Not found"));
                return responseModel;
            }

            var accountOwner = CreateBankAccountOwner(bankAccount);

            responseModel.Data = CreateRecipientBankAccountResponse(bankAccount, accountOwner);

            return responseModel;
        }


        /// <summary>
        /// Add or Edit bank account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddOrEditBankAccountAsync(int accountId, CreateBankAccountRequest request,
                                                                 CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            if (accountId > 0)
            {
                var bankaccount = await _bankAccountRepo.FindByIdAsync(accountId);

                if (bankaccount != null)
                {
                    bankaccount.IBAN = request.IBAN;
                    bankaccount.CurrencyId = request.CurrencyId;
                    bankaccount.Balance = request.Balance;
                    bankaccount.Type = request.Type;
                    bankaccount.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                    bankaccount.LastModifiedOn = DateTime.UtcNow;

                    await _bankAccountRepo.UpdateAsync(bankaccount);
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError("bankAccount", $"bank account Not found"));
                    return responseModel;
                }
            }
            else
            {
                try
                {
                    var newBankAccount = CreateBankAccount(request);

                    await _bankAccountRepo.AddAsync(newBankAccount);
                }
                catch (Exception ex)
                {

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
        public async Task<ApiResponse> ActivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _bankAccountRepo.FindByIdAsync(accountId);

            if (bankAccount != null)
            {
                bankAccount.Disabled = false;
            }
            else
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account id: {accountId} not found"));
            }

            return responseModel;
        }


        /// <summary>
        /// Deactivate bank account for for the specified id
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> DeactivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _bankAccountRepo.FindByIdAsync(accountId);

            if (bankAccount != null)
            {
                bankAccount.Disabled = true;
            }
            else
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount)));
            }

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

            var bankAccount = await _bankAccountRepo.FindByIdAsync(accountId);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank account not found"));
            }

            if (bankAccount.Type == AccountType.Savings)
            {
                var deposits = await _cashTransactionsRepo.GetDepositsByIBAN(bankAccount.IBAN);

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
                                responseModel.AddError(ExceptionCreator.CreateInternalServerError());

                                return responseModel;
                                }

                            break;
                            

                         case "USD":

                                try
                                {
                                if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 186 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 365)
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
                                responseModel.AddError(ExceptionCreator.CreateInternalServerError());

                                return responseModel;
                            }

                                break;

                        case "EUR":

                                try
                                {
                                    if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 186 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 365)
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
                                responseModel.AddError(ExceptionCreator.CreateInternalServerError());

                                return responseModel;
                                }

                                break;

                        default:
                            return responseModel;

                    }

                   
                    profit = deposit.Amount * (decimal)interestRate;
                    bankAccount.Balance += deposit.Amount + profit;                   
                }
            }

            return responseModel;
        }


        #region private helper methods
        private BankAccountResponse CreateBankAccountResponse(BankAccount bankAccount, string accountOwner, DateTime? lastTransactionDate = null)
        {
            if (bankAccount != null)
            {
                return new BankAccountResponse(bankAccount.Id, bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type,
                                               accountOwner,bankAccount.Branch.Code, bankAccount.Branch.Name,
                                               bankAccount.Balance, bankAccount.AllowedBalanceToUse,
                                               bankAccount.Currency.Name, bankAccount.CreatedAt, lastTransactionDate);
            }

            return null;
        }

        private RecipientBankAccountResponse CreateRecipientBankAccountResponse(BankAccount bankAccount, string accountOwner)
        {
            if (bankAccount != null)
            {
                return new RecipientBankAccountResponse(bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type,
                                                        accountOwner, bankAccount.Branch.Name, bankAccount.Currency.Name);
            }

            return null;
        }

        private BankAccount CreateBankAccount(CreateBankAccountRequest request)
        {

            var newBankAccount = new BankAccount()
            {
                AccountNo = Guid.NewGuid().ToString(),
                IBAN = request.IBAN,
                CustomerId = request.CustomerId,
                BranchId = request.BranchId,
                Balance = request.Balance,
                CurrencyId = request.CurrencyId,
                Type = request.Type,
                CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
            };


                return newBankAccount;
        }

        private string CreateBankAccountOwner(BankAccount bankAccount)
        {
            return bankAccount.Owner.FirstName + " " + bankAccount.Owner.LastName;
        }

      
        #endregion

    }
}
