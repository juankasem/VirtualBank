using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BankAccountService(VirtualBankDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<BankAccountListResponse>> GetAccountsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountListResponse>();

            var bankAccounts = await _dbContext.BankAccounts.Where(a => a.CustomerId == customerId && a.Disabled == false).ToListAsync();

            var bankAccountList = new List<BankAccountResponse>();

            foreach (var bankAccount in bankAccounts)
            {
                var accountOwner = bankAccount.Owner.FirstName + " " + bankAccount.Owner.LastName;
                var lastTransaction = await GetLastCashTransaction(bankAccount);

                bankAccountList.Add(CreateBankAccountResponse(bankAccount, accountOwner, lastTransaction.CreatedOn));
            }

            responseModel.Data = new BankAccountListResponse(bankAccountList.ToImmutableList(), bankAccountList.Count);

            return responseModel;
 
        }

        public async Task<ApiResponse<BankAccountResponse>> GetAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.AccountNo == accountNo && a.Disabled == false);

            if (bankAccount == null)
            {
                responseModel.AddError($"bank account No: {accountNo} Not found");
                return responseModel;
            }

            var accountOwner = bankAccount.Owner.FirstName + " " + bankAccount.Owner.LastName;
            var lastTransaction = await GetLastCashTransaction(bankAccount);

            responseModel.Data = CreateBankAccountResponse(bankAccount, accountOwner, lastTransaction.CreatedOn);

            return responseModel;
        }

        public async Task<ApiResponse<BankAccountResponse>> GetAccountByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.IBAN == iban && a.Disabled == false);

            if (bankAccount == null)
            {
                responseModel.AddError($"IBAN: {iban} Not found");
                return responseModel;
            }

            var accountOwner = bankAccount.Owner.FirstName + " " + bankAccount.Owner.LastName;
            var lastTransaction = await _dbContext.CashTransactions.Where(c => c.From == bankAccount.AccountNo || c.To == bankAccount.AccountNo)
                                                                   .OrderByDescending(c => c.CreatedOn).FirstOrDefaultAsync();

            responseModel.Data = CreateBankAccountResponse(bankAccount, accountOwner, lastTransaction.CreatedOn);

            return responseModel;
        }


        public async Task<ApiResponse<RecipientBankAccountResponse>> GetRecipientAccountByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<RecipientBankAccountResponse>();

            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.IBAN == iban && a.Disabled == false);

            if (bankAccount == null)
            {
                responseModel.AddError($"IBAN: {iban} Not found");
                return responseModel;
            }

            var accountOwner = bankAccount.Owner.FirstName + " " + bankAccount.Owner.LastName;

            responseModel.Data = CreateRecipientBankAccountResponse(bankAccount, accountOwner);

            return responseModel;
        }


        public async Task<ApiResponse> AddOrEditBankAccountAsync(int accountId, CreateBankAccountRequest request,
                                                                 CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();
            var user = _httpContextAccessor.HttpContext.User;
            var bankaccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId);



            if (bankaccount != null)
            {
                bankaccount.IBAN = request.Account.IBAN;
                bankaccount.Currency = request.Account.Currency;
                bankaccount.Balance = request.Account.Balance;
                bankaccount.Type = request.Account.Type;
                bankaccount.LastModifiedBy = user.Identity.Name;
                bankaccount.LastModifiedOn = DateTime.UtcNow;
            }
            else
            {
                var newBankAccount = CreateBankAccount(request);

                if (newBankAccount == null)
                {
                    responseModel.AddError("couldn't create new bank account");
                    return responseModel;
                }

                newBankAccount.CreatedBy = user.Identity.Name;

                await _dbContext.BankAccounts.AddAsync(newBankAccount);
            }

            await _dbContext.SaveChangesAsync();

            return responseModel;
        }

        public async Task<ApiResponse> ActivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId);

            if (bankAccount != null)
            {
                bankAccount.Disabled = false;
            }
            else
            {
                responseModel.AddError($"bank account Not found");
            }

            return responseModel;
        }

        public async Task<ApiResponse> DeactivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId);

            if (bankAccount != null)
            {
                bankAccount.Disabled = true;
            }
            else
            {
                responseModel.AddError($"bank account Not found");
            }

            return responseModel;
        }

        public async Task<ApiResponse<BankAccountResponse>> CalculateNetProfits(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankAccount = await _dbContext.BankAccounts.Include("Currency").FirstOrDefaultAsync(a => a.Id == accountId);

            if (bankAccount == null)
            {
               responseModel.AddError($"bank account Not found");
            }

            if (bankAccount.Type == AccountType.Savings)
            {

                var deposits = await GetBankAccountDeposits(bankAccount.IBAN);

                foreach (var deposit in deposits)
                {
                    decimal profit = 0;
                    double profitRate = 0.00;

                    switch (bankAccount.Currency.Code)
                    {
                        case "TL":
                            
                                try
                                {
                                    if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 186 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 365)
                                    {
                                        profitRate = 0.17;
                                    }
                                    else if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays > 365 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 720)
                                    {
                                        profitRate = 0.19;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    responseModel.AddError(ex.ToString());

                                    return responseModel;
                                }

                            break;
                            

                         case "USD":

                                try
                                {
                                if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 186 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 365)
                                {
                                    profitRate = 0.15;
                                }
                                else if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays > 365 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 720)
                                {
                                    profitRate = 0.17;
                                }
                            }
                                catch (Exception ex)
                                {
                                responseModel.AddError(ex.ToString());

                                return responseModel;
                            }

                                break;

                        case "EUR":

                                try
                                {
                                    if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays >= 186 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 365)
                                    {
                                        profitRate = 0.15;
                                    }
                                    else if (DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays > 365 && DateTime.UtcNow.Subtract(deposit.TransactionDate).TotalDays <= 720)
                                    {
                                        profitRate = 0.17;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    responseModel.AddError(ex.ToString());

                                    return responseModel;
                                }

                                break;

                        default:
                            return responseModel;

                    }

                   
                    profit = deposit.Amount * (decimal)profitRate;
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
                                               bankAccount.Currency.Name, bankAccount.CreatedOn, lastTransactionDate);
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
            var bankAccount = request.Account;

            if (bankAccount != null)
            {
                var newBankAccount = new BankAccount()
                {
                    AccountNo = Guid.NewGuid().ToString() + bankAccount.Branch.Code,
                    CustomerId = bankAccount.CustomerId,
                    BranchId = bankAccount.BranchId,
                    Balance = bankAccount.Balance,
                    Currency = bankAccount.Currency,
                    Type = bankAccount.Type,                    
                };

                return newBankAccount;
            }

            return null;
        }

        private async Task<List<CashTransaction>> GetBankAccountDeposits(string iban)
        {
            var deposits = await _dbContext.CashTransactions.Where(c => c.To == iban).ToListAsync();

            return deposits;
        }

        private async Task<CashTransaction> GetLastCashTransaction(BankAccount bankAccount)
        {
            return await _dbContext.CashTransactions.Where(c => c.From == bankAccount.AccountNo || c.To == bankAccount.AccountNo)
                                                                    .OrderByDescending(c => c.CreatedOn).FirstOrDefaultAsync();
        }
        #endregion

    }
}
