using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountriesService(VirtualBankDbContext dbContext,
                                ICountriesRepository countriesRepo,
                                IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _countriesRepo = countriesRepo;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ApiResponse<CountriesResponse>> GetAllCountriesAsync(CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CountriesResponse>();

            var countries = await _countriesRepo.GetAll();

            var countryList = new List<CountryResponse>();

            foreach (var country in countries)
            {
                countryList.Add(CreateCountryResponse(country));
            }

            responseModel.Data = new CountriesResponse(countryList.ToImmutableList(), countryList.Count);

            return responseModel;
        }

       
        public async Task<ApiResponse<CountryResponse>> GetCountryByIdAsync(int countryId, bool includeCities, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CountryResponse>();

            Country country= null;

            if (includeCities)
                country = await _countriesRepo.FindByIdWithCitiesAsync(countryId);

            else
                country = await _countriesRepo.FindByIdAsync(countryId);


            if (country == null)
            {
                responseModel.AddError("country not found");
                return responseModel;
            }

            if (includeCities)
             responseModel.Data = CreateCountryWithCitiesResponse(country);

            else
            responseModel.Data = CreateCountryResponse(country);

            return responseModel;
        }


        public async Task<ApiResponse> AddOrEditCountryAsync(int countryId, CreateCountryRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            if (await CountryNameExists(request.Name))
            {
                responseModel.AddError("country name does already exist");
                return responseModel;
            }

            var user = _httpContextAccessor.HttpContext.User;
            var country = await _countriesRepo.FindByIdAsync(countryId);

            if (country != null)
            {
                country.Name = request.Name;
                country.Code = request.Code;
                country.LastModifiedBy = user.Identity.Name;
                country.LastModifiedOn = DateTime.UtcNow;

                await _countriesRepo.UpdateAsync(country);
            }
            else
            {
                var newCountry = CreateCountry(request);

                if (newCountry == null)
                {
                    responseModel.AddError("couldn't create new country");
                    return responseModel;
                }

                newCountry.CreatedBy = user.Identity.Name;

                await _countriesRepo.AddAsync(newCountry);
            }


            return responseModel;
        }


        public async Task<bool> CountryExists(int countryId)
        {
            return await _dbContext.Countries.AnyAsync(c => c.Id == countryId);
        }

        public async Task<bool> CountryNameExists(string countryName)
        {
            return await _dbContext.Countries.AnyAsync(c => c.Name == countryName);
        }

        #region private helper methods
        private Country CreateCountry(CreateCountryRequest request)
        {
            if (request != null)
            {
                return new Country()
                {
                    Name = request.Name,
                    Code = request.Code,
                }; 
            }

            return null;
        }

        private CountryResponse CreateCountryResponse(Country country)
        {
            if (country != null)
            {
                return new CountryResponse(country.Id, country.Name, country.Code, null);
            }

            return null;
        }

        private CountryResponse CreateCountryWithCitiesResponse(Country country)
        {
            if (country != null)
            {
                var cityList = country.Cities.ToList();
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
