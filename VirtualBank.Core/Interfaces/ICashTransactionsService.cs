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

        Task<ApiResponse<CashTransactionListResponse>> GetAllCashTransactionsAsync(int pageNumber, int pageSize,
                                                                                   CancellationToken cancellationToken = default);

        Task<ApiResponse<CashTransactionListResponse>> GetBankAccountCashTransactionsAsync(string iban, int lastDays,
                                                                                           int pageNumber, int pageSize,
                                                                                           CancellationToken cancellationToken = default);


        Task<ApiResponse<CashTransactionResponse>> GetLastCashTransactionAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse> MakeDepositAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> MakeWithdrawalAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> MakeTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> MakeEFTTransferAsync(CreateCashTransactionRequest request, CancellationToken cancellationToken = default);
    }
}
