using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface ICustomersService
    {
        Task<ApiResponse<CustomerResponse>> GetCustomerByAccountNo(string accountNo, CancellationToken cancellationToken);

        Task<ApiResponse> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken);

        Task<ApiResponse> DeactivateCustomer(string customerId);
    }
}
