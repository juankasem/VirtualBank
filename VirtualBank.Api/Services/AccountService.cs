using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public class AccountService : IAccountService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public AccountService(VirtualBankDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ApiResponse<AccountsResponse>> GetAccountsByCustomerIdAsync(string customerId, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse<AccountsResponse>();

            var accountsList = await _dbContext.Accounts.Where(a => a.CustomerId == customerId).ToListAsync();

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

            var account = await _dbContext.Accounts.Where(a => a.AccountNo == accountNo).FirstOrDefaultAsync();

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

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNo == accountNo);

            if (account != null)
            {
                account.IBAN = request.Account.IBAN;
                account.Type = request.Account.Type;
                account.ModifiedBy = request.Account.Owner.User;
            }
            else
            {
                var newAccount = CreateAccount(request);

                if (newAccount == null)
                {
                    responseModel.AddError("couldn't create new account");
                    return responseModel;
                }

                await _dbContext.Accounts.AddAsync(newAccount);
            }

            await _dbContext.SaveChangesAsync();

            return responseModel;
        }

        public async Task<ApiResponse> DeactivateAccountAsync(string accountId, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse<AccountResponse>();

            var account = await _dbContext.Accounts.Where(a => a.Id == accountId).FirstOrDefaultAsync();

            if (account == null)
            {
                responseModel.AddError($"Account Not found");
                return responseModel;
            }

            responseModel.Data = new AccountResponse(account);

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
                    CreatedBy = account.CreatedBy,
                    Type = account.Type,
                };

                return newAccount;
            }

            return null;
        }
    }
}
