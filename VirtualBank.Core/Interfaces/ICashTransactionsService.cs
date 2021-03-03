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
        Task<ApiResponse<CashTransactionsResponse>> GetAllCashTransactionsByIBANAsync(string iban, int lastDays, CancellationToken cancellationToken = default);

        Task<ApiResponse<CashTransactionsResponse>> GetFastCashTransactionsByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<CashTransactionResponse>> GetLastCashTransactionAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddCashTransactionAsync(CreateCashTransactionRequest request,  CancellationToken cancellationToken = default);
    }
}
