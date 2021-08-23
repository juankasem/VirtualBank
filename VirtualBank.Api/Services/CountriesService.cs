using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CityApiResponses;
using VirtualBank.Core.ApiResponseModels.CountryApiResponse;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountriesService(IUnitOfWork unitOfWork,
                                IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieve all countries
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CountryListResponse>> GetAllCountriesAsync(bool includeCities = false, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CountryListResponse>();

            var countries = await _unitOfWork.Countries.GetAllAsync();

            if (!countries.Any())
            {
                return responseModel;
            }

            ImmutableList<CountryResponse> countryList;

            if (includeCities)
                countryList = countries.OrderBy(country => country.Name)
                                       .Select(country => CreateCountryWithCitiesResponse(country).Result)
                                       .ToImmutableList();

            else
                countryList = countries.OrderBy(country => country.Name)
                                       .Select(country => CreateCountryResponse(country))
                                       .ToImmutableList();


            responseModel.Data = new CountryListResponse(countryList, countryList.Count);

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
                country = await _unitOfWork.Countries.FindByIdWithCitiesAsync(countryId);

            else
                country = await _unitOfWork.Countries.FindByIdAsync(countryId);


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
        public async Task<ApiResponse<CountryResponse>> AddOrEditCountryAsync(int countryId, CreateCountryRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CountryResponse>();

            if (await _unitOfWork.Countries.CountryNameExistsAsync(request.Name))
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError("country", "country name does already exist"));
                return responseModel;
            }

            if (countryId != 0)
            {
                var country = await _unitOfWork.Countries.FindByIdAsync(countryId);

                if (country != null)
                {
                    country.Name = request.Name;
                    country.Code = request.Code;
                    country.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                    country.LastModifiedOn = DateTime.UtcNow;

                    var updatedCountry = await _unitOfWork.Countries.UpdateAsync(country);
                    responseModel.Data = CreateCountryResponse(updatedCountry);

                    await _unitOfWork.CompleteAsync();
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(country), $"country of id {countryId}: not found"));
                    return responseModel;
                }
            }
            else
            {
                try
                {
                    var addedCountry = await _unitOfWork.Countries.AddAsync(CreateCountry(request));

                    responseModel.Data = CreateCountryResponse(addedCountry);

                    await _unitOfWork.CompleteAsync();
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                }
            }

            return responseModel;
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
                    CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
                };
            }

            return null;
        }

        private static CountryResponse CreateCountryResponse(Country country)
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
                var cities = await _unitOfWork.Cities.GetByCountryIdAsync(country.Id);

                var cityList = cities.OrderBy(c => c.Name).Select(c => CreateCityResponse(c)).ToImmutableList();

                return new CountryResponse(country.Id, country.Name, country.Code, cityList);
            }

            return null;
        }

        private static CityResponse CreateCityResponse(City city)
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
