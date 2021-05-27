using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.DebitCardApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.DebitCardApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IDebitCardsService
    {
        Task<ApiResponse<DebitCardListResponse>> GetAllDebitCardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<DebitCardResponse>> GetDebitCardByIdAsync(int debitCardId, CancellationToken cancellationToken = default);

        Task<ApiResponse<DebitCardResponse>> GetDebitCardByDebitCardNoAsync(string debitCardNo, CancellationToken cancellationToken = default);

        Task<ApiResponse<DebitCardResponse>> GetDebitCardByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<bool> ValidateDebitCardPINAsync(string debitCardNo, string pin, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditDebitCardAsync(int debitCardId, CreateDebitCardRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> ActivateDebitCardAsync(int debitCardId, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeactivateDebitCardAsync(int debitCardId, CancellationToken cancellationToken = default);
    }
}
