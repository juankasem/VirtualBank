using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.CityApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CityApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CitiesService : ICitiesService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly ICitiesRepository _citiesRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CitiesService(VirtualBankDbContext dbContext,
                             ICitiesRepository citiesRepo,
                             IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _citiesRepo = citiesRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieve all cities
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CityListResponse>> GetAllCitiesAsync(CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CityListResponse>();

            var allCities = await _citiesRepo.GetAllAsync();

            if (!allCities.Any())
            {
                return responseModel;
            }

            var cities = allCities.OrderBy(c => c.Name);

            var cityList = new List<CityResponse>();

            foreach (var city in cities)
            {
                cityList.Add(CreateCityResponse(city));
            }

            responseModel.Data = new CityListResponse(cityList.ToImmutableList(), cityList.Count);

            return responseModel;
        }


        /// <summary>
        ///  Retrieve cities for the country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CityListResponse>> GetCitiesByCountryIdAsync(int countryId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CityListResponse>();

            var countryCities = await _citiesRepo.GetByCountryIdAsync(countryId);

            if (!countryCities.Any())
            {
                return responseModel;
            }

            var cities = countryCities.OrderBy(c => c.Name);

            var cityList = new List<CityResponse>();

            foreach (var city in cities)
            {
                cityList.Add(CreateCityResponse(city));
            }

            responseModel.Data = new CityListResponse(cityList.ToImmutableList(), cityList.Count);

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

            var city = await _citiesRepo.FindByIdAsync(cityId);

            if (city == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(city), $"city of id {cityId}: not found"));

                return responseModel;
            }

            responseModel.Data = CreateCityResponse(city);

            return responseModel;
        }


        /// <summary>
        /// Add or Edit an existing city
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddOrEditCityAsync(int cityId, CreateCityRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            if (await _citiesRepo.CityNameExists(request.CountryId, request.Name))
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError("city", "city name does already exist"));
                return responseModel;
            }

            if (cityId != 0)
            {
                var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == cityId);

                try
                {
                    if (city != null)
                    {
                        city.CountryId = request.CountryId;
                        city.Name = request.Name;
                        city.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                        city.LastModifiedOn = DateTime.UtcNow;

                        await _citiesRepo.UpdateAsync(city);
                    }
                    else
                    {
                        responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(city), $"city of Id: { cityId} not found"));
                        return responseModel;
                    }
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                }
            }

            else
            {
                try
                {
                    await _citiesRepo.AddAsync(CreateCity(request));
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
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
            return await _citiesRepo.CityExists(cityId);
        }   


        #region private helper methods
        private City CreateCity(CreateCityRequest request)
        {
            return new City()
            {
                CountryId = request.CountryId,
                Name = request.Name,
                CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
            };
        }

        private CityResponse CreateCityResponse(City city)
        {
            if (city != null)
            {
                return new CityResponse(city.Id, city.CountryId, city.Name);
            }

            return null;
        }

        #endregion
    }
}
