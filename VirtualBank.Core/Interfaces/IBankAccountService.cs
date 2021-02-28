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
        Task<ApiResponse<BankAccountsResponse>> GetAccountsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> GetAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> GetAccountByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<RecipientBankAccountResponse>> GetRecipientAccountByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse> CreateOrUpdateBankAccountAsync(string accountNo, CreateBankAccountRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> ActivateBankAccountAsync(string accountId, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeactivateBankAccountAsync(string accountId, CancellationToken cancellationToken = default);
    }
}
