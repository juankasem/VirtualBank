using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiRequestModels.RecipientApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IBankAccountService
    {
        Task<ApiResponse<BankAccountListResponse>> GetAccountsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> GetAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> GetAccountByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<RecipientBankAccountResponse>> GetRecipientAccountByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditBankAccountAsync(int accountId, CreateBankAccountRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> ActivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeactivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> CalculateNetProfits(int accountId, CancellationToken cancellationToken = default);
    }
}
