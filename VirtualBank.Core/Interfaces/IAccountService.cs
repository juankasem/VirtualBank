using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IAccountsService
    {
        Task<ApiResponse<AccountsResponse>> GetAccountsByCustomerId(string customerId, CancellationToken cancellationToken);

        Task<ApiResponse<AccountResponse>> GetAccountByAccountNo(string accountNo, CancellationToken cancellationToken);

        Task<ApiResponse> CreateOrUpdateAccount(string accountNo, CreateAccountRequest request, CancellationToken cancellationToken);

        Task<ApiResponse> DeactivateAccount(string accountId, CancellationToken cancellationToken);
    }
}
