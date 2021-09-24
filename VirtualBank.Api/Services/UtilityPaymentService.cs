using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Core.ApiRequestModels.UtilityPaymentApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.UtilityPaymentApiResponses;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class UtilityPaymentService : IUtilityPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UtilityPaymentService(IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }


        public Task<ApiResponse<UtilityPaymentListResponse>> GetAllUtilityPaymentsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiResponse<UtilityPaymentResponse>> GetUtilityPaymentByIdsync(int id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiResponse<UtilityPaymentListResponse>> GetUtilityPaymentsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiResponse<UtilityPaymentListResponse>> GetUtilityPaymentsByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> AddOrEditUtilityPaymentAsync(int UtilityPaymentId, CreateUtilityPaymentRequest request, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}