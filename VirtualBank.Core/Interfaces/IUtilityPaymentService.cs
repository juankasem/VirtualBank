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

        Task<ApiResponse<UtilityPaymentListResponse>> GetUtilityPaymentsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse<UtilityPaymentListResponse>> GetUtilityPaymentsByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<UtilityPaymentResponse>> GetUtilityPaymentByIdsync(Guid id, CancellationToken cancellationToken = default);

        Task<Response> AddOrEditUtilityPaymentAsync(Guid UtilityPaymentId, CreateUtilityPaymentRequest request, CancellationToken cancellationToken = default);
    }
}
