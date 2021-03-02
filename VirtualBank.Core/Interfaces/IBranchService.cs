using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.BranchApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.BranchApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IBranchService
    {
        Task<ApiResponse<BranchesResponse>> GetAllBranches(CancellationToken cancellationToken = default);

        Task<ApiResponse<BranchesResponse>> GetBranchesByCityId(int cityId, CancellationToken cancellationToken = default);

        Task<ApiResponse<BranchResponse>> GetBranchByCode(string code, CancellationToken cancellationToken = default);

        Task<ApiResponse> AddOrEditBranch(string code, CreateBranchRequest request, CancellationToken cancellationToken = default);
    }
}
