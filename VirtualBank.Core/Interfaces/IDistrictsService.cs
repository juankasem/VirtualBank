using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.DistrictApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.DistrictApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IDistrictsService
    {
        Task<ApiResponse<DistrictListResponse>> GetAllDistrictsAsync(CancellationToken cancellationToken = default);

        Task<ApiResponse<DistrictListResponse>> GetDistrictsByCityIdAsync(int cityId, CancellationToken cancellationToken = default);

        Task<ApiResponse<DistrictResponse>> GetDistrictByIdAsync(int districtId, CancellationToken cancellationToken = default);

        Task<Response> AddOrEditDistrictAsync(int districtId, CreateDistrictRequest request, CancellationToken cancellationToken = default);

        Task<bool> DistrictExists(int cityId);
    }
}
