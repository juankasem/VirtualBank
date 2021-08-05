using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.LoanApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.LoanApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface ILoansService
    {
        Task<ApiResponse<LoanListResponse>> GetAllLoansAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<LoanListResponse>> GetLoansByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse<LoanListResponse>> GetLoansByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<LoanResponse>> GetLoanByIdsync(int id, CancellationToken cancellationToken = default);

        Task<Response> AddOrEditLoanAsync(int loanId, CreateLoanRequest request,  CancellationToken cancellationToken = default);
    }
}
