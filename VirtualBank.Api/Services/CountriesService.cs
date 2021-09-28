using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CountryApiResponse;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Models.Responses;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICountryMapper _countryMapper;
        private readonly ICityMapper _cityMapper;


        public CountriesService(IUnitOfWork unitOfWork,
                                ICountryMapper countryMapper,
                                ICityMapper cityMapper)
        {
            _unitOfWork = unitOfWork;
            _countryMapper = countryMapper;
            _cityMapper = cityMapper;
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

            ImmutableList<Country> countryList;

            if (includeCities)
                countryList = countries.OrderBy(country => country.Name)
                                       .Select(country => _countryMapper.MapToResponseModel(country,
                                       _unitOfWork.Cities.GetByCountryIdAsync(country.Id).Result.OrderBy(c => c.Name).Select(city => ToCountryCity(city))
                                       .ToImmutableList()))
                                       .ToImmutableList();

            else
                countryList = countries.OrderBy(country => country.Name)
                                       .Select(country => _countryMapper.MapToResponseModel(country))
                                       .ToImmutableList();


            responseModel.Data = new(countryList, countryList.Count);

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

            Core.Entities.Country country = null;

            if (includeCities)
                country = await _unitOfWork.Countries.FindByIdWithCitiesAsync(countryId);

            else
                country = await _unitOfWork.Countries.FindByIdAsync(countryId);


            if (country == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(country), $"country of id {countryId}: not found"));
                return responseModel;
            }

            //TODO: Retrieve list of country cities
            if (includeCities)
            {
                var cities =
                responseModel.Data = new(_countryMapper.MapToResponseModel(country,
                _unitOfWork.Cities.GetByCountryIdAsync(country.Id).Result.OrderBy(c => c.Name)
                                                                         .Select(city => ToCountryCity(city))
                                                                         .ToImmutableList()));
            }
            else
                responseModel.Data = new(_countryMapper.MapToResponseModel(country));

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
                    country.LastModifiedBy = request.ModificationInfo.ModifiedBy;
                    country.LastModifiedOn = request.ModificationInfo.LastModifiedOn;

                    try
                    {
                        var updatedCountry = await _unitOfWork.Countries.UpdateAsync(country);
                        responseModel.Data = new(_countryMapper.MapToResponseModel(updatedCountry));

                        await _unitOfWork.SaveAsync();
                    }
                    catch (Exception ex)
                    {
                        responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                    }
                }
                else
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(country), $"country of id {countryId}: not found"));

            }
            else
            {
                try
                {
                    var createdCountry = await _unitOfWork.Countries.AddAsync(CreateCountry(request));

                    responseModel.Data = new(_countryMapper.MapToResponseModel(createdCountry));

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


        #region private helper methods
        private Core.Entities.Country CreateCountry(CreateCountryRequest request)
        {
            if (request != null)
            {
                return new()
                {
                    Name = request.Name,
                    Code = request.Code,
                    CreatedBy = request.CreationInfo.CreatedBy,
                };
            }

            return null;
        }

        private static Country.City ToCountryCity(Core.Entities.City city) =>
         new(city.Id, city.Name);

        #endregion
    }
}
