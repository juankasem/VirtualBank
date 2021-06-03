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

        Task<ApiResponse<FastTransactionListResponse>> GetBankAccountFastTransactionsAsync(string iban, int pageNumber, int pageSize,
                                                                                       CancellationToken cancellationToken = default);

        Task<ApiResponse<FastTransactionResponse>> GetFastTransactionByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Response> AddOrEditFastTransactionAsync(int id, CreateFastTransactionRequest request, CancellationToken cancellationToken = default);

        Task<Response> DeleteFastTransactionAsync(int id, CancellationToken cancellationToken = default);
    }
}
