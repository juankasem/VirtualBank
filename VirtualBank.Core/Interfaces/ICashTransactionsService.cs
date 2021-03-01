using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface ICashTransactionsService
    {
        Task<ApiResponse<CashTransactionsResponse>> GetAllCashTransactionsByAccountNoAsync(string accountNo, int lastDays, CancellationToken cancellationToken = default);

        Task<ApiResponse<CashTransactionsResponse>> GetFastCashTransactionsByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<ApiResponse<CashTransactionResponse>> GetLastCashTransactionAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddCashTransactionAsync(CreateCashTransactionRequest request,  CancellationToken cancellationToken = default);

        Task<ApiResponse> DeleteCashTransactionAsync(string cashTransactionId, CancellationToken cancellationToken = default);
    }
}
