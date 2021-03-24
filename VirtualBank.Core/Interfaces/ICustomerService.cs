﻿using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface ICustomerService
    {
        Task<ApiResponse<CustomerListResponse>> GetAllCustomersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse<CustomerResponse>> GetCustomerByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default);

        Task<ApiResponse<CustomerResponse>> GetCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse<RecipientCustomerResponse>> GetRecipientCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditCustomerAsync(int customerId, CreateCustomerRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> ActivateCustomerAsync(int customerId, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeactivateCustomerAsync(int customerId, CancellationToken cancellationToken = default);

        Task<bool> CustomerExists(CreateCustomerRequest request);
    }
}
