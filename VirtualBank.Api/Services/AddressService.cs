using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.AddressApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;
using VirtualBank.Core.Interfaces;

namespace VirtualBank.Api.Services
{
    public class AddressService : IAddressService
    {
        public AddressService()
        {
        }

        public Task<ApiResponse> AddOrEditAddressAsync(int cityId, CreateAddressRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int addressId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
