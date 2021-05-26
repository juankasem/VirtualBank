using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IBankAccountService
    {
        Task<ApiResponse<BankAccountListResponse>> GetBankAccountsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> GetBankAccountByIdAsync(int accountId, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> GetBankAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> GetBankAccountByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<RecipientBankAccountResponse>> GetRecipientBankAccountByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditBankAccountAsync(int accountId, CreateBankAccountRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> ActivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeactivateBankAccountAsync(int accountId, CancellationToken cancellationToken = default);

        Task<ApiResponse<BankAccountResponse>> CalculateNetProfits(int accountId, CancellationToken cancellationToken = default);
    }
}
