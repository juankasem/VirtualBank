using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.UtilityPaymentApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.UtilityPaymentApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IUtilityPaymentService
    {
        Task<ApiResponse<UtilityPaymentListResponse>> GetAllUtilityPaymentsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<UtilityPaymentListResponse>> GetUtilityPaymentsByIBANAsync(string iban, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<UtilityPaymentResponse>> GetUtilityPaymentByIdsync(Guid utilityPaymentId, CancellationToken cancellationToken = default);

        Task<Response> AddOrEditUtilityPaymentAsync(Guid utilityPaymentId, CreateUtilityPaymentRequest request, CancellationToken cancellationToken = default);
    }
}
