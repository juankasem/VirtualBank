using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Api.Helpers.ErrorsHelper;
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

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAddressRepository _addressRepository;

        public AddressService(IHttpContextAccessor httpContextAccessor,
                              IAddressRepository addressRepository)
        {
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

            var allAddresses = await _addressRepository.GetAllAsync();

            if (allAddresses.Count() == 0)
            {
                return responseModel;
            }

            var addresses = allAddresses.OrderBy(a => a.CreatedAt).Skip((pageNumber - 1) * pageSize)
                                                                  .Take(pageSize);

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
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(address), $"address id: {addressId} not found"));

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
        public async Task<Response> AddOrEditAddressAsync(int addressId, CreateAddressRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new Response();

           
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
                    address.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                    address.LastModifiedOn = DateTime.UtcNow;

                    await _addressRepository.UpdateAsync(address);
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(address), $"address id: {addressId} not found"));

                    return responseModel;
                }
            }
            else
            {
                if (await AddressExistsAsync(request.CountryId, request.CityId, request.DistrictId, request.Street, request.Name))
                {
                    responseModel.AddError(ExceptionCreator.CreateBadRequestError("address", "naddress name does already exist"));

                    return responseModel;
                }

                try
                {
                    await _addressRepository.AddAsync(CreateAddress(request));
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                }
            }

            return responseModel;
        }

        /// <summary>
        /// Disable address
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> DeleteAddressAsync(int addressId, CancellationToken cancellationToken = default)
        {
            var responseModel = new Response();

            var address = await _addressRepository.FindByIdAsync(addressId);

            if (address == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(address), $"address id: {addressId} not found"));

                return responseModel;
            }

            try
            {
                await _addressRepository.RemoveAsync(address.Id);
            }
            catch (Exception ex)
            {
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
            }

            return responseModel;
        }

        /// <summary>
        /// Check if address exists
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="cityId"></param>
        /// <param name="districtId"></param>
        /// <param name="street"></param>
        /// <returns></returns>
        public async Task<bool> AddressExistsAsync(int countryId, int cityId, int districtId, string street, string name)
        {
            return await _addressRepository.AddressExistsAsync(countryId, cityId, districtId, street, name);
        }



        #region private helper methods
        private Address CreateAddress(CreateAddressRequest request)
        {
            return new Address()
            {
                Name = request.Name,
                DistrictId = request.DistrictId,
                CityId = request.CityId,
                CountryId = request.CountryId,
                Street = request.Street,
                PostalCode = request.PostalCode,
                CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
                };
           
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
