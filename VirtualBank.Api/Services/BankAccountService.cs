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
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BankAccountService(VirtualBankDbContext dbContext,
                                  UserManager<AppUser> userManager,
                                  IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<AccountsResponse>> GetAccountsByCustomerIdAsync(string customerId, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse<AccountsResponse>();

            var accountsList = await _dbContext.Accounts.Where(a => a.CustomerId == customerId && a.Disabled == false).ToListAsync();

            var accounts = new ImmutableArray<Account>();

            foreach (var account in accountsList)
            {
                accounts.Add(account);
            }

            responseModel.Data = new AccountsResponse(accounts);

            return responseModel;
 
        }

        public async Task<ApiResponse<AccountResponse>> GetAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse<AccountResponse>();

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNo == accountNo && a.Disabled == false);

            if (account == null)
            {
                responseModel.AddError($"Account No: {accountNo} Not found");
                return responseModel;
            }

            responseModel.Data = new AccountResponse(account);

            return responseModel;
        }

        public async Task<ApiResponse> CreateOrUpdateAccountAsync(string accountNo, CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse();
            var user = _httpContextAccessor.HttpContext.User;
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNo == accountNo);

            if (account != null)
            {
                account.IBAN = request.Account.IBAN;
                account.Currency = request.Account.Currency;
                account.Balance = request.Account.Balance;
                account.Type = request.Account.Type;
                account.ModifiedBy = await _userManager.GetUserAsync(user);
                account.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                var newAccount = CreateAccount(request);

                if (newAccount == null)
                {
                    responseModel.AddError("couldn't create new account");
                    return responseModel;
                }

                newAccount.CreatedBy = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

                await _dbContext.Accounts.AddAsync(newAccount);
            }

            await _dbContext.SaveChangesAsync();

            return responseModel;
        }

        public async Task<ApiResponse> ActivateAccountAsync(string accountId, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse<AccountResponse>();

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);

            if (account != null)
            {
                account.Disabled = false;
            }
            else
            {
                responseModel.AddError($"Account Not found");

            }

            return responseModel;
        }

        public async Task<ApiResponse> DeactivateAccountAsync(string accountId, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse<AccountResponse>();

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);

            if (account != null)
            {
                account.Disabled = true;
            }
            else
            {
                responseModel.AddError($"Account Not found");

            }

            return responseModel;
        }


        private Account CreateAccount(CreateAccountRequest request)
        {
            var account = request.Account;

            if (account != null)
            {
                var newAccount = new Account()
                {
                    AccountNo = Guid.NewGuid().ToString() + account.Branch.Code,
                    CustomerId = account.CustomerId,
                    BranchId = account.BranchId,
                    Balance = account.Balance,
                    Currency = account.Currency,
                    Type = account.Type,                    
                };

                return newAccount;
            }

            return null;
        }

        
    }
}
