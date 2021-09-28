using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.CityApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CityApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CitiesService : ICitiesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICityMapper _cityMapper;
        public CitiesService(IUnitOfWork unitOfWork,
                             ICityMapper cityMapper)
        {
            _unitOfWork = unitOfWork;
            _cityMapper = cityMapper;
        }

        /// <summary>
        /// Retrieve all cities
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CityListResponse>> ListCitiesAsync(int countryId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CityListResponse>();

            IEnumerable<City> cities;

            if (countryId > 0)
            {
                cities = await _unitOfWork.Cities.GetByCountryIdAsync(countryId);
            }
            else
            {
                cities = await _unitOfWork.Cities.GetAllAsync();
            }

            if (!cities.Any())
                return responseModel;


            var cityList = cities.OrderBy(c => c.Name).Select(city => _cityMapper.MapToResponseModel(city)).ToImmutableList();


            responseModel.Data = new(cityList, cityList.Count);

            return responseModel;
        }

        /// <summary>
        /// Retrieve city for the specified id
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="includeDistricts"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CityResponse>> GetCityByIdAsync(int cityId, bool includeDistricts = true, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CityResponse>();

            var city = await _unitOfWork.Cities.FindByIdAsync(cityId);

            if (city == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(city), $"city of id {cityId}: not found"));

                return responseModel;
            }

            responseModel.Data = new(_cityMapper.MapToResponseModel(city));

            return responseModel;
        }


        /// <summary>
        /// Add or Edit an existing city
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CityResponse>> AddOrEditCityAsync(int cityId, CreateCityRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CityResponse>();

            if (await _unitOfWork.Cities.CityNameExists(request.CountryId, request.Name))
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError("city", "city name does already exist"));
                return responseModel;
            }

            if (cityId != 0)
            {
                var city = await _unitOfWork.Cities.FindByIdAsync(cityId);

                try
                {
                    if (city != null)
                    {
                        city.CountryId = request.CountryId;
                        city.Name = request.Name;
                        city.LastModifiedBy = request.ModificationInfo.ModifiedBy;
                        city.LastModifiedOn = request.ModificationInfo.LastModifiedOn;

                        var updatedCity = await _unitOfWork.Cities.UpdateAsync(city);

                        responseModel.Data = new(_cityMapper.MapToResponseModel(updatedCity));

                        await _unitOfWork.SaveAsync();
                    }
                    else
                    {
                        responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(city), $"city of Id: { cityId} not found"));
                    }
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                    return responseModel;
                }
            }
            else
            {
                try
                {
                    var createdCity = await _unitOfWork.Cities.AddAsync(CreateCity(request));

                    responseModel.Data = new(_cityMapper.MapToResponseModel(createdCity));

                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                    return responseModel;
                }
            }

            return responseModel;
        }


        /// <summary>
        /// Check if city name already 
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public async Task<bool> CityExists(int cityId)
        {
            return await _unitOfWork.Cities.CityExists(cityId);
        }


        #region private helper methods
        private City CreateCity(CreateCityRequest request) =>
           new()
           {
               CountryId = request.CountryId,
               Name = request.Name,
               CreatedBy = request.CreationInfo.CreatedBy,
               CreatedOn = request.CreationInfo.CreatedOn
           };

        #endregion
    }
}
