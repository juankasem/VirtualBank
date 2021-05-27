using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.CreditCardApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CreditCardApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface ICreditCardsService
    {
        Task<ApiResponse<CreditCardListResponse>> GetAllCreditCardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<CreditCardResponse>> GetCreditCardByIdAsync(int creditCardId, CancellationToken cancellationToken = default);

        Task<ApiResponse<CreditCardResponse>> GetCreditCardByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<bool> ValidateCreditCardPINAsync(string creditCardNo, string pin, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditCreditCardAsync(int creditCardId, CreateCreditCardRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> ActivateCreditCardAsync(int creditCardId, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeactivateCreditCardAsync(int creditCardId, CancellationToken cancellationToken = default);
    }
}
