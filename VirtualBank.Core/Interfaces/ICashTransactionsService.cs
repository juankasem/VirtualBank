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
        Task<ApiResponse<CashTransactionsResponse>> GetCashTransactionsByAccountNo(string accountNo, int lastDays, CancellationToken cancellationToken = default);

        Task<ApiResponse<CashTransactionsResponse>> GetCashTransactionsByCustomerNo(string customerNo, int lastDays, CancellationToken cancellationToken = default);

        Task<ApiResponse> CreateCashTransaction(CreateCashTransactionRequest request,  CancellationToken cancellationToken = default);

        Task<ApiResponse> DeleteCashTransaction(string cashTransactionId, CancellationToken cancellationToken = default);
    }
}
