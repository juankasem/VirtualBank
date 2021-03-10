using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.CityApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CityApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class CitiesService : ICitiesService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CitiesService(VirtualBankDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
     
        public async Task<ApiResponse<CitiesResponse>> GetAllCitiesAsync(CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CitiesResponse>();

            var cityList = await _dbContext.Cities.OrderBy(c => c.Name).ToListAsync();

            var cities = new List<CityResponse>();

            foreach (var city in cityList)
            {
                cities.Add(CreateCityResponse(city));
            }

            responseModel.Data = new CitiesResponse(cities.ToImmutableArray());

            return responseModel;
        }

        public async Task<ApiResponse<CitiesResponse>> GetCitiesByCountryIdAsync(int countryId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CitiesResponse>();

            var cityList = await _dbContext.Cities.Where(c => c.CountryId == countryId).OrderBy(c => c.Name).ToListAsync();

            var cities = new List<CityResponse>();

            foreach (var city in cityList)
            {
                cities.Add(CreateCityResponse(city));
            }

            responseModel.Data = new CitiesResponse(cities.ToImmutableArray());

            return responseModel;
        }

        public async Task<ApiResponse<CityResponse>> GetCityByIdAsync(int cityId, bool includeDistricts = true, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CityResponse>();

            var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == cityId);

            if (city == null)
            {
                responseModel.AddError("city not found");
                return responseModel;
            }

            responseModel.Data = CreateCityResponse(city);

            return responseModel;
        }

        public async Task<ApiResponse> AddOrEditCityAsync(int cityId, CreateCityRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            var user = _httpContextAccessor.HttpContext.User;
            var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.Id == cityId);

            if (city != null)
            {
                city.Name = request.Name;
                city.ModifiedBy = user.Identity.Name;
                city.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                var newCity = CreateCity(request);

                if (newCity == null)
                {
                    responseModel.AddError("couldn't create new city");
                    return responseModel;
                }

                newCity.CreatedBy = user.Identity.Name;

                await _dbContext.Cities.AddAsync(newCity);
            }

            await _dbContext.SaveChangesAsync();

            return responseModel;
        }

        public async Task<bool> CityExists(int cityId)
        {
            return await _dbContext.Cities.AnyAsync(c => c.Id == cityId);
        }

        #region private helper methods
        private City CreateCity(CreateCityRequest request)
        {
            if (request != null)
            {
                return new City()
                {
                    CountryId = request.CountryId,
                    Name = request.Name,
                };
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
