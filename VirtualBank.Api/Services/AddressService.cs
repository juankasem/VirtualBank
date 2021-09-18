using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.AddressApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class AddressService : IAddressService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IHttpContextAccessor httpContextAccessor,
                              IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
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

            var allAddresses = await _unitOfWork.Addresses.GetAllAsync();

            if (allAddresses.Any())
            {
                return responseModel;
            }

            var addressList = allAddresses.OrderBy(a => a.CreatedOn).Skip((pageNumber - 1) * pageSize)
                                                                    .Take(pageSize)
                                                                    .Select(x => CreateAddressResponse(x))
                                                                    .ToImmutableList();


            responseModel.Data = new AddressListResponse(addressList, addressList.Count);

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

            var address = await _unitOfWork.Addresses.FindByIdAsync(addressId);

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
        public async Task<ApiResponse<AddressResponse>> AddOrEditAddressAsync(int addressId, CreateAddressRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<AddressResponse>();

            if (addressId != 0)
            {
                var address = await _unitOfWork.Addresses.FindByIdAsync(addressId);

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

                    var updatedAddress = await _unitOfWork.Addresses.UpdateAsync(address);
                    responseModel.Data = CreateAddressResponse(updatedAddress);

                    await _unitOfWork.SaveAsync();

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
                    var createdAddress = await _unitOfWork.Addresses.AddAsync(CreateAddress(request));
                    responseModel.Data = CreateAddressResponse(createdAddress);

                    await _unitOfWork.SaveAsync();
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

            var address = await _unitOfWork.Addresses.FindByIdAsync(addressId);

            if (address == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(address), $"address id: {addressId} not found"));

                return responseModel;
            }

            try
            {
                await _unitOfWork.Addresses.RemoveAsync(address.Id);
                await _unitOfWork.SaveAsync();
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
            return await _unitOfWork.Addresses.AddressExistsAsync(countryId, cityId, districtId, street, name);
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
                    PostalCode = request.PostalCode,
                    CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
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
