using System;
using System.Collections.Generic;
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

      
        public async Task<ApiResponse<BranchListResponse>> GetAllBranchesAsync(CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchListResponse>();

            var branchList = await _dbContext.Branches.ToListAsync();

            var branches = new List<BranchResponse>();

            foreach (var branch in branchList)
            {
                branches.Add(CreateBranchResponse(branch));
            }

            responseModel.Data = new BranchListResponse(branches.ToImmutableList(), branches.Count);

            return responseModel;
        }

        public async Task<ApiResponse<BranchListResponse>> GetBranchesByCityIdAsync(int cityId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchListResponse>();

            var branchesList = await _dbContext.Branches.Include(b => b.Address.CityId == cityId).ToListAsync();

            var branches = new List<BranchResponse>();

            foreach (var branch in branchesList)
            {
                branches.Add(CreateBranchResponse(branch));
            }

            responseModel.Data = new BranchListResponse(branches.ToImmutableList(), branches.Count);

            return responseModel;
        }

        public async Task<ApiResponse<BranchResponse>> GetBranchByIdAsync(int branchId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchResponse>();

            var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == branchId);


            responseModel.Data = CreateBranchResponse(branch);

            return responseModel;
        }

        public async Task<ApiResponse<BranchResponse>> GetBranchByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchResponse>();

            var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Code == code);


            responseModel.Data = CreateBranchResponse(branch);

            return responseModel;
        }

        public async Task<ApiResponse> AddOrEditBranchAsync(int branchId, CreateBranchRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();
            var user = _httpContextAccessor.HttpContext.User;
            var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == branchId);

            if (branch != null)
            {
                branch.Name = request.Name;
                branch.Code = request.Code;
                branch.Address = request.Address;
                branch.LastModifiedBy = user.Identity.Name;
                branch.LastModifiedOn = DateTime.UtcNow;
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
            if (request != null)
            {
                return new Branch()
                {
                    Name = request.Name,
                    Code = request.Code,
                    Phone = request.Phone,
                    Address = request.Address,
                };
            }

            return null;
        }

        private BranchResponse CreateBranchResponse(Branch branch)
        {
            if (branch != null)
            {
                return new BranchResponse(branch.Id, branch.Name, branch.Code, branch.Phone, branch.Address);
            }

            return null;
        }

        #endregion
    }
}
