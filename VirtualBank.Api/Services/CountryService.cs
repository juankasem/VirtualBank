using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.BranchApiRequests;
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CountryApiResponse;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class CountryService : ICountryService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountryService(VirtualBankDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ApiResponse<CountriesResponse>> GetAllCountries(CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CountriesResponse>();

            var countryList = await _dbContext.Countries.ToListAsync();

            var countries = new ImmutableArray<CountryResponse>();

            foreach (var country in countryList)
            {
                countries.Add(CreateCountryResponse(country));
            }

            responseModel.Data = new CountriesResponse(countries);

            return responseModel;
        }

       
        public async Task<ApiResponse<CountryResponse>> GetCountryById(int countryId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CountryResponse>();

            var country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Id == countryId);

            responseModel.Data = CreateCountryResponse(country);

            return responseModel;
        }

        public async Task<ApiResponse> AddOrEditCountry(int counryId, CreateCountryRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();
            var user = _httpContextAccessor.HttpContext.User;
            var country = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Id == counryId);

            if (country != null)
            {
                country.Name = request.Name;
                country.Code = request.Code;
                country.ModifiedBy = user.Identity.Name;
                country.ModifiedOn = DateTime.UtcNow;
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

                await _dbContext.Countries.AddAsync(newCountry);
            }

            await _dbContext.SaveChangesAsync();

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
                }; 
            }

            return null;
        }

        private CountryResponse CreateCountryResponse(Country country)
        {
            if (country != null)
            {
                return new CountryResponse(country.Name, country.Code);
            }

            return null;
        }

        #endregion
    }
}
