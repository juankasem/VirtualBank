using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IFastTransactionsService
    {
        Task<ApiResponse<FastTransactionListResponse>> GetAllFastTransactionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<FastTransactionListResponse>> GetAccountFastTransactionsAsync(int accountId, int pageNumber, int pageSize,
                                                                                       CancellationToken cancellationToken = default);

        Task<ApiResponse<FastTransactionResponse>> GetFastTransactionByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditFastTransactionAsync(int id, CreateFastTransactionRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeleteFastTransactionAsync(int id, CancellationToken cancellationToken = default);
    }
}
