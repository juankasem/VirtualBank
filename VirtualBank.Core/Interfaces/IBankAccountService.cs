using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IBankAccountService
    {
        Task<ApiResponse<AccountsResponse>> GetAccountsByCustomerIdAsync(string customerId, CancellationToken cancellationToken);

        Task<ApiResponse<AccountResponse>> GetAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken);

        Task<ApiResponse> CreateOrUpdateAccountAsync(string accountNo, CreateAccountRequest request, CancellationToken cancellationToken);

        Task<ApiResponse> ActivateAccountAsync(string accountId, CancellationToken cancellationToken);

        Task<ApiResponse> DeactivateAccountAsync(string accountId, CancellationToken cancellationToken);
    }
}
