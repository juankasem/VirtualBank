using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.RecipientApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.RecipientApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IRecipientService
    {
        Task<ApiResponse<RecipientListResponse>> GetBankAccountRecipientsAsync(int accountId, CancellationToken cancellationToken = default);

        Task<ApiResponse<RecipientResponse>> GetRecipientbyIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditRecipientAsync(CreateRecipientRequest request, CancellationToken cancellationToken = default);
    }
}
