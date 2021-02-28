using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly ICashTransactionsService _cashTransactionsService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BankAccountService(VirtualBankDbContext dbContext,
                                  ICashTransactionsService cashTransactionsService,
                                  UserManager<AppUser> userManager,
                                  IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _cashTransactionsService = cashTransactionsService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<BankAccountsResponse>> GetAccountsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountsResponse>();

            var bankAccountsList = await _dbContext.BankAccounts.Where(a => a.CustomerId == customerId && a.Disabled == false).ToListAsync();

            var bankAccounts = new ImmutableArray<BankAccountResponse>();

            foreach (var bankAccount in bankAccountsList)
            {
                var accountOwner = bankAccount.Owner.FirstName + " " + bankAccount.Owner.LastName;
                var branchName = bankAccount.Branch.Name;
                var lastTransaction = await GetLastCashTransaction(bankAccount);

                bankAccounts.Add(CreateBankAccountResponse(bankAccount, accountOwner, branchName, lastTransaction.CreatedOn));
            }

            responseModel.Data = new BankAccountsResponse(bankAccounts);

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
            var branchName = bankAccount.Branch.Name;
            var lastTransaction = await GetLastCashTransaction(bankAccount);

            responseModel.Data = CreateBankAccountResponse(bankAccount, accountOwner, branchName, lastTransaction.CreatedOn);

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
            var branchName = bankAccount.Branch.Name;
            var lastTransaction = await _dbContext.CashTransactions.Where(c => c.From == bankAccount.AccountNo || c.To == bankAccount.AccountNo)
                                                                   .OrderByDescending(c => c.CreatedOn).FirstOrDefaultAsync();

            responseModel.Data = CreateBankAccountResponse(bankAccount, accountOwner, branchName, lastTransaction.CreatedOn);

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
            var branchName = bankAccount.Branch.Name;


            responseModel.Data = CreateRecipientBankAccountResponse(bankAccount, accountOwner, branchName);

            return responseModel;
        }


        public async Task<ApiResponse> CreateOrUpdateBankAccountAsync(string accountNo, CreateBankAccountRequest request,
                                                                      CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();
            var user = _httpContextAccessor.HttpContext.User;
            var bankaccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.AccountNo == accountNo);

            if (bankaccount != null)
            {
                bankaccount.IBAN = request.Account.IBAN;
                bankaccount.Currency = request.Account.Currency;
                bankaccount.Balance = request.Account.Balance;
                bankaccount.Type = request.Account.Type;
                bankaccount.ModifiedBy = user.Identity.Name;
                bankaccount.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                var newBankAccount = CreateBankAccount(request);

                if (newBankAccount == null)
                {
                    responseModel.AddError("couldn't create new bankaccount");
                    return responseModel;
                }

                newBankAccount.CreatedBy = user.Identity.Name;

                await _dbContext.BankAccounts.AddAsync(newBankAccount);
            }

            await _dbContext.SaveChangesAsync();

            return responseModel;
        }

        public async Task<ApiResponse> ActivateBankAccountAsync(string accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BankAccountResponse>();

            var bankaccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.Id == accountId);

            if (bankaccount != null)
            {
                bankaccount.Disabled = false;
            }
            else
            {
                responseModel.AddError($"bank account Not found");

            }

            return responseModel;
        }

        public async Task<ApiResponse> DeactivateBankAccountAsync(string accountId, CancellationToken cancellationToken = default)
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


        #region
        private BankAccountResponse CreateBankAccountResponse(BankAccount bankAccount, string accountOwner, string branchName, DateTime? lastTransactionDate = null)
        {
            if (bankAccount != null)
            {
                return new BankAccountResponse(bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type,
                                               accountOwner, branchName, bankAccount.Balance, bankAccount.AllowedBalanceToUse,
                                               bankAccount.Currency.Name, bankAccount.CreatedOn, lastTransactionDate);
            }

            return null;
        }

        private RecipientBankAccountResponse CreateRecipientBankAccountResponse(BankAccount bankAccount, string accountOwner, string branchName)
        {
            if (bankAccount != null)
            {
                return new RecipientBankAccountResponse(bankAccount.AccountNo, bankAccount.IBAN, bankAccount.Type, accountOwner,
                                                        branchName, bankAccount.Currency.Name);
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

        private async Task<CashTransaction> GetLastCashTransaction(BankAccount bankAccount)
        {
            return await _dbContext.CashTransactions.Where(c => c.From == bankAccount.AccountNo || c.To == bankAccount.AccountNo)
                                                                    .OrderByDescending(c => c.CreatedOn).FirstOrDefaultAsync();
        }
        #endregion

    }
}
