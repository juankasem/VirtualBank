using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.AddressApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IAddressService
    {
        Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int addressId, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditAddressAsync(int cityId, CreateAddressRequest request, CancellationToken cancellationToken = default);
    }
}
