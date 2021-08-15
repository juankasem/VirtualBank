﻿using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.UtilityPaymentApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.UtilityPaymentApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IUtilityPaymentInterface
    {
        Task<ApiResponse<UtilityPaymentListResponse>> GetAllUtilityPaymentsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<UtilityPaymentListResponse>> GetUtilityPaymentsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse<UtilityPaymentListResponse>> GetUtilityPaymentsByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<UtilityPaymentResponse>> GetUtilityPaymentByIdsync(int id, CancellationToken cancellationToken = default);

        Task<Response> AddOrEditUtilityPaymentAsync(int UtilityPaymentId, CreateUtilityPaymentRequest request, CancellationToken cancellationToken = default);
    }
}
