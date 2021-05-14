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
        Task<ApiResponse<AddressListResponse>> GetAllAddressesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int addressId, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditAddressAsync(int addressId, CreateAddressRequest request, CancellationToken cancellationToken = default);

        Task<ApiResponse> DeleteAddressAsync(int addressId, CancellationToken cancellationToken = default);

        Task<bool> AddressExistsAsync(int countryId, int cityId, int districtId, string street, string name);
    }
}
