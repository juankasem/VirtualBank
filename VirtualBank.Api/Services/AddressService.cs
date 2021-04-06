using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.AddressApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class AddressService : IAddressService
    {

        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAddressRepository _addressRepository;

        public AddressService(VirtualBankDbContext dbContext,
                             IHttpContextAccessor httpContextAccessor,
                             IAddressRepository addressRepository)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _addressRepository = addressRepository;
        }

        /// <summary>
        /// Retrieve all addreses
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<AddressListResponse>> GetAllAddressesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<AddressListResponse>();
            var skip = (pageNumber - 1) * pageSize;

            var allAddresses = await _addressRepository.GetAll();
            var addresses = allAddresses.OrderBy(a => a.CreatedOn).Skip(skip).Take(pageSize);

            var addressList = new List<AddressResponse>();

            foreach (var address in addresses)
            {
                addressList.Add(CreateAddressResponse(address));
            }

            responseModel.Data = new AddressListResponse(addressList.ToImmutableList(), addressList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve the address of the specified id
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int addressId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<AddressResponse>();

            var address = await _addressRepository.FindByIdAsync(addressId);

            if (address == null)
            {
                responseModel.AddError($"address {addressId} not found");
                return responseModel;
            }


            responseModel.Data = CreateAddressResponse(address);

            return responseModel;
        }


        /// <summary>
        /// Add or Edit an existing address
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddOrEditAddressAsync(int addressId, CreateAddressRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            if (await AddressExists(request.CountryId, request.CityId, request.DistrictId, request.Street))
            {
                responseModel.AddError("address name does already exist");
                return responseModel;
            }

            var user = _httpContextAccessor.HttpContext.User;

            if (addressId != 0)
            {
                var address = await _addressRepository.FindByIdAsync(addressId);

                if (address != null)
                {
                    address.Name = request.Name;
                    address.CountryId = request.CountryId;
                    address.CityId = request.CityId;
                    address.DistrictId = request.DistrictId;
                    address.Street = request.Street;
                    address.PostalCode = request.PostalCode;
                    address.LastModifiedBy = user.Identity.Name;
                    address.LastModifiedOn = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    responseModel.AddError("address not found");
                    return responseModel;
                }
            }
            else
            {
                var newAddress = CreateAddress(request);

                if (newAddress == null)
                {
                    responseModel.AddError("couldn't create new address");
                    return responseModel;
                }

                newAddress.CreatedBy = user.Identity.Name;
      
                try
                {
                    await _addressRepository.AddAsync(newAddress);
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ex.ToString());
                }
            }

            return responseModel;
        }

        public async Task<bool> AddressExists(int countryId, int cityId, int districtId, string street)
        {
            return await _dbContext.Addresses.AnyAsync(a => a.CountryId == countryId && a.CityId == cityId &&
                                                            a.DistrictId == districtId && a.Name.Equals(street) );
        }

        #region private helper methods
        private Address CreateAddress(CreateAddressRequest request)
        {
            if (request != null)
            {
                return new Address()
                {
                    Name = request.Name,
                    DistrictId = request.DistrictId,
                    CityId = request.CityId,
                    CountryId = request.CountryId,
                    Street = request.Street,
                    PostalCode = request.PostalCode
                };
            }

            return null;
        }

        private AddressResponse CreateAddressResponse(Address address)
        {
            if (address != null)
            {
                return new AddressResponse(address.Id, address.Name,
                                           address.DistrictId, address.District.Name,
                                           address.CityId, address.City.Name,
                                           address.CountryId, address.Country.Name,
                                           address.Street, address.PostalCode);
            }

            return null;
        }

      
        #endregion
    }
}
