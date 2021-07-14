using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.CityApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CityApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface ICitiesService
    {
        Task<ApiResponse<CityListResponse>> ListCitiesAsync(int countryId, CancellationToken cancellationToken = default);

        Task<ApiResponse<CityResponse>> GetCityByIdAsync(int cityId, bool includeDistricts = true, CancellationToken cancellationToken = default);

        Task<ApiResponse<CityResponse>> AddOrEditCityAsync(int cityId, CreateCityRequest request, CancellationToken cancellationToken = default);

        Task<bool> CityExists(int cityId);
    }
}
