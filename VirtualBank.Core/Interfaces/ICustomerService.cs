using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface ICustomerService
    {
        Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(string customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse<CustomerResponse>> GetCustomerByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<ApiResponse<CustomerResponse>> GetCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<RecipientCustomerResponse>> GetRecipientCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditCustomerAsync(string customerId, CreateCustomerRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> ActivateCustomerAsync(string customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeactivateCustomerAsync(string customerId, CancellationToken cancellationToken = default);
    }
}
