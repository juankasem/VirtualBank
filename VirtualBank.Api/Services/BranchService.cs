using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.BranchApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.BranchApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{

    public class BranchService : IBranchService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BranchService(VirtualBankDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

      
        public async Task<ApiResponse<BranchesResponse>> GetAllBranches(CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchesResponse>();

            var branchesList = await _dbContext.Branches.ToListAsync();

            var branches = new ImmutableArray<BranchResponse>();

            foreach (var branch in branchesList)
            {
                branches.Add(CreateBranchResponse(branch));
            }

            responseModel.Data = new BranchesResponse(branches);

            return responseModel;
        }

        public async Task<ApiResponse<BranchesResponse>> GetBranchesByCityId(int cityId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchesResponse>();

            var branchesList = await _dbContext.Branches.Where(b => b.Address.CityId == cityId).ToListAsync();

            var branches = new ImmutableArray<BranchResponse>();

            foreach (var branch in branchesList)
            {
                branches.Add(CreateBranchResponse(branch));
            }

            responseModel.Data = new BranchesResponse(branches);

            return responseModel;
        }

        public async Task<ApiResponse<BranchResponse>> GetBranchByCode(string code, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchResponse>();

            var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Code == code);


            responseModel.Data = CreateBranchResponse(branch);

            return responseModel;
        }

        public async Task<ApiResponse> AddOrEditBranch(string code, CreateBranchRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();
            var user = _httpContextAccessor.HttpContext.User;
            var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Code == code);

            if (branch != null)
            {
                branch.Name = request.Branch.Name;
                branch.Code = request.Branch.Code;
                branch.Address = request.Branch.Address;
                branch.ModifiedBy = user.Identity.Name;
                branch.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                var newBranch = CreateBranch(request);

                if (newBranch == null)
                {
                    responseModel.AddError("couldn't create new branch");
                    return responseModel;
                }

                newBranch.CreatedBy = user.Identity.Name;

                await _dbContext.Branches.AddAsync(newBranch);
            }

            await _dbContext.SaveChangesAsync();

            return responseModel;
        }


        #region private helper methods
        private Branch CreateBranch(CreateBranchRequest request)
        {
            var branch = request.Branch;

            if (branch != null)
            {
                var newBranch = new Branch()
                {
                    Name = request.Branch.Name,
                    Code = request.Branch.Code,
                    Address = request.Branch.Address,
                };

                return newBranch;
            }

            return null;
        }

        private BranchResponse CreateBranchResponse(Branch branch)
        {
            if (branch != null)
            {
                return new BranchResponse(branch.Name, branch.Code, branch.Address);
            }

            return null;
        }

        #endregion
    }
}
