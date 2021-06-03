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
        Task<ApiResponse<BranchListResponse>> GetAllBranchesAsync(int pageNumber, int pageSize,
                                                                  CancellationToken cancellationToken = default);

        Task<ApiResponse<BranchListResponse>> GetBranchesByCityIdAsync(int cityId, int pageNumber, int pageSize,
                                                                       CancellationToken cancellationToken = default);

        Task<ApiResponse<BranchResponse>> GetBranchByIdAsync(int branchId, CancellationToken cancellationToken = default);

        Task<ApiResponse<BranchResponse>> GetBranchByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<Response> AddOrEditBranchAsync(int branchId, CreateBranchRequest request,
                                              CancellationToken cancellationToken = default);


        Task<Response> DeleteBranchAsync(int branchId, CancellationToken cancellationToken = default);

        Task<bool> BranchExists(int countryId, int cityId, string branchName);
    }
}
