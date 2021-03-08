using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.BranchApiRequests;
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CountryApiResponse;

namespace VirtualBank.Core.Interfaces
{
    public interface ICountryService
    {
        Task<ApiResponse<CountriesResponse>> GetAllCountries(CancellationToken cancellationToken = default);

        Task<ApiResponse<CountryResponse>> GetCountryById(int countryId, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditCountry(int counryId, CreateCountryRequest request, CancellationToken cancellationToken = default);
    }
}
