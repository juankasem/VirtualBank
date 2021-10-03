using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.BranchApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.BranchApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{

    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBranchMapper _branchMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BranchService(IUnitOfWork unitOfWork,
                             IBranchMapper branchMapper,
                             IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _branchMapper = branchMapper;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieve all branches
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchListResponse>> ListBranchesAsync(int countryId, int cityId, int districtId,
                                                                             int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchListResponse>();

            var branches = await _unitOfWork.Branches.ListAsync(countryId, cityId, districtId);

            if (!branches.Any())
            {
                return responseModel;
            }

            var branchList = branches.OrderByDescending(b => b.CreatedOn).Skip((pageNumber - 1) * pageSize)
                                                                         .Take(pageSize)
                                                                         .Select(branch => _branchMapper.MapToResponseModel(branch))
                                                                         .ToImmutableList();

            responseModel.Data = new(branchList, branchList.Count);

            return responseModel;
        }


        /// <summary>
        /// search branches by name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchListResponse>> SearchBranchesByNameAsync(string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchListResponse>();

            var searchResult = await _unitOfWork.Branches.SearchByNameAsync(searchTerm);

            if (!searchResult.Any())
            {
                return responseModel;
            }

            var branchList = searchResult.OrderByDescending(b => b.CreatedOn).Skip((pageNumber - 1) * pageSize)
                                                                             .Take(pageSize)
                                                                             .Select(branch => _branchMapper.MapToResponseModel(branch))
                                                                             .ToImmutableList();

            responseModel.Data = new BranchListResponse(branchList, branchList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve the branch of the specified id
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchResponse>> GetBranchByIdAsync(int branchId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchResponse>();

            var branch = await _unitOfWork.Branches.FindByIdAsync(branchId);

            if (branch == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(branch), $"branch of Id: {branchId} not found"));
                return responseModel;
            }

            responseModel.Data = new(_branchMapper.MapToResponseModel(branch));

            return responseModel;
        }


        /// <summary>
        /// Retrieve the branch by the specified code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchResponse>> GetBranchByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchResponse>();

            var branch = await _unitOfWork.Branches.FindByCodeAsync(code);

            if (branch == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(branch), $"branch code: {code} not found"));
                return responseModel;
            }

            responseModel.Data = new(_branchMapper.MapToResponseModel(branch));

            return responseModel;
        }


        /// <summary>
        /// Add or Edit an existing branch 
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchResponse>> AddOrEditBranchAsync(int branchId, CreateBranchRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchResponse>();

            if (await BranchExists(request.Address.CountryId, request.Address.CityId, request.Name))
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError("branch", $"branch name: {request.Name} already exists"));

                return responseModel;
            }

            if (branchId != 0)
            {
                var branch = await _unitOfWork.Branches.FindByIdAsync(branchId);

                if (branch != null)
                {
                    branch.Name = request.Name;
                    branch.Code = request.Code;
                    branch.Address = request.Address;
                    branch.LastModifiedBy = request.CreationInfo.CreatedBy;
                    branch.LastModifiedOn = request.CreationInfo.CreatedOn;

                    try
                    {
                        var updatedBranch = await _unitOfWork.Branches.UpdateAsync(branch);
                        await _unitOfWork.SaveAsync();

                        responseModel.Data = new(_branchMapper.MapToResponseModel(updatedBranch));
                    }
                    catch (Exception ex)
                    {
                        responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                        return responseModel;
                    }
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(branch), $"branch of Id: {branchId} not found"));
                }
            }
            else
            {
                try
                {
                    var createdBranch = await _unitOfWork.Branches.AddAsync(CreateBranch(request));

                    responseModel.Data = new(_branchMapper.MapToResponseModel(createdBranch));

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

        /// <summary>
        /// disable branch
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> DeleteBranchAsync(int branchId, CancellationToken cancellationToken = default)
        {
            var responseModel = new Response();

            var branch = await _unitOfWork.Branches.FindByIdAsync(branchId);

            if (branch == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(branch)));
                return responseModel;
            }

            try
            {
                bool isDeleted = await _unitOfWork.Branches.RemoveAsync(branch.Id);

                if (!isDeleted)
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError("Unexpected error"));
            }
            catch (Exception ex)
            {
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
            }

            return responseModel;
        }


        /// <summary>
        /// Check if branch exists
        /// </summary>
        /// <param name="branchName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BranchExists(int countryId, int cityId, string branchName, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Branches.ExistsAsync(countryId, cityId, branchName);
        }


        /************************************************************Private Methods*******************************************************/
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
                    Address = CreateAddress(request),
                    CreatedBy = request.CreationInfo.CreatedBy,
                    CreatedOn = request.CreationInfo.CreatedOn,
                    LastModifiedBy = request.CreationInfo.CreatedBy,
                    LastModifiedOn = request.CreationInfo.CreatedOn
                };
            }

            return null;
        }

        private Address CreateAddress(CreateBranchRequest request)
        {
            if (request != null)
            {
                var address = request.Address;

                return new(address.Name,
                            address.DistrictId,
                            address.CityId,
                            address.CountryId,
                            address.Street != null ? address.Street : null,
                            address.PostalCode != null ? address.PostalCode : null,
                            request.CreationInfo.CreatedBy,
                            request.CreationInfo.CreatedOn,
                            request.CreationInfo.CreatedBy,
                            request.CreationInfo.CreatedOn);
            }

            return null;
        }

        #endregion
    }
}
