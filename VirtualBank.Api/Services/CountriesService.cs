using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CityApiResponses;
using VirtualBank.Core.ApiResponseModels.CountryApiResponse;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly ICountriesRepository _countriesRepo;
        private readonly ICitiesRepository _citiesRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountriesService(VirtualBankDbContext dbContext,
                                ICountriesRepository countriesRepo,
                                ICitiesRepository  citiesRepo,
                                IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _countriesRepo = countriesRepo;
            _citiesRepo = citiesRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieve all countries
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CountriesResponse>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CountriesResponse>();

            var countries = await _countriesRepo.GetAllAsync();

            var countryList = new List<CountryResponse>();

            foreach (var country in countries)
            {
                countryList.Add(CreateCountryResponse(country));
            }

            responseModel.Data = new CountriesResponse(countryList.ToImmutableList(), countryList.Count);

            return responseModel;
        }

        /// <summary>
        /// Retrieve country for the specified id
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="includeCities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CountryResponse>> GetCountryByIdAsync(int countryId, bool includeCities = false, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CountryResponse>();

            Country country = null;

            if (includeCities)
                country = await _countriesRepo.FindByIdWithCitiesAsync(countryId);

            else
                country = await _countriesRepo.FindByIdAsync(countryId);


            if (country == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(country), $"country of id {countryId}: not found"));
                return responseModel;
            }

            if (includeCities)
             responseModel.Data = await CreateCountryWithCitiesResponse(country);

            else
            responseModel.Data = CreateCountryResponse(country);

            return responseModel;
        }


        /// <summary>
        /// Add Or Edit country
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddOrEditCountryAsync(int countryId, CreateCountryRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            if (await CountryNameExists(request.Name))
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError("country", "country name does already exist"));
                return responseModel;
            }

            if (countryId != 0)
            {
                var country = await _countriesRepo.FindByIdAsync(countryId);

                if (country != null)
                {
                    country.Name = request.Name;
                    country.Code = request.Code;
                    country.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                    country.LastModifiedOn = DateTime.UtcNow;

                    await _countriesRepo.UpdateAsync(country);
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(country), $"country of id {countryId}: not found"));
                    return responseModel;
                }
            }
         
            else
            {
                var newCountry = CreateCountry(request);

                await _countriesRepo.AddAsync(newCountry);
            }

            return responseModel;
        }


        /// <summary>
        /// Check whetehr country exists or not
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<bool> CountryExists(int countryId)
        {
            return await _dbContext.Countries.AnyAsync(c => c.Id == countryId);
        }

        /// <summary>
        /// Check whetehr country's name is alraedy used or not
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public async Task<bool> CountryNameExists(string countryName)
        {
            return await _dbContext.Countries.AnyAsync(c => c.Name == countryName);
        }


        #region private helper methods
        private Country CreateCountry(CreateCountryRequest request)
        {
            return new Country()
            {
                Name = request.Name,
                Code = request.Code,
                CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
            }; 

        }

        private CountryResponse CreateCountryResponse(Country country)
        {
            if (country != null)
            {
                return new CountryResponse(country.Id, country.Name, country.Code, null);
            }

            return null;
        }

        private async Task<CountryResponse> CreateCountryWithCitiesResponse(Country country)
        {
            if (country != null)
            {
                var cityList = await _citiesRepo.GetByCountryIdAsync(country.Id);
                var cities = new List<CityResponse>();

                foreach (var city in cityList)
                {
                    cities.Add(CreateCityResponse(city));
                }

                return new CountryResponse(country.Id, country.Name, country.Code, cities.ToImmutableList());
            }

            return null;
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
