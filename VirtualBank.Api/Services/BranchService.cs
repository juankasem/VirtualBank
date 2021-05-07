﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.BranchApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;
using VirtualBank.Core.ApiResponseModels.BranchApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{

    public class BranchService : IBranchService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBranchRepository _branchRepo;
        private readonly IAddressRepository _addressRepo;

        public BranchService(VirtualBankDbContext dbContext,
                             IHttpContextAccessor httpContextAccessor,
                             IBranchRepository branchRepo,
                             IAddressRepository addressRepo)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _branchRepo = branchRepo;
            _addressRepo = addressRepo;
        }

        /// <summary>
        /// Retrieve all branches
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchListResponse>> GetAllBranchesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchListResponse>();

            var allBranches = await _branchRepo.GetAllAsync();
            var branches = allBranches.OrderByDescending(b => b.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var branchList = new List<BranchResponse>();

            foreach (var branch in branches)
            {
                var address = await _addressRepo.FindByIdAsync(branch.AddressId);
                branchList.Add(CreateBranchResponse(branch, address));
            }

            responseModel.Data = new BranchListResponse(branchList.ToImmutableList(), branchList.Count);

            return responseModel;
        }

        /// <summary>
        /// Retrieve the branches of the specified city
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchListResponse>> GetBranchesByCityIdAsync(int cityId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchListResponse>();
            var skip = (pageNumber - 1) * pageSize;

            var cityBranches = await _branchRepo.GetByCityIdAsync(cityId);
            var branches = cityBranches.OrderByDescending(b => b.CreatedAt).Skip(skip).Take(pageSize);

            var branchList = new List<BranchResponse>();

            foreach (var branch in branches)
            {
                var address = await _addressRepo.FindByIdAsync(branch.AddressId);
                branchList.Add(CreateBranchResponse(branch, address));
            }

            responseModel.Data = new BranchListResponse(branchList.ToImmutableList(), branchList.Count);

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

            var branch = await _branchRepo.FindByIdAsync(branchId);

            if (branch == null)
            {
                responseModel.AddError($"branch {branchId} not found");
                return responseModel;
            }

            var address = await _addressRepo.FindByIdAsync(branch.AddressId);

            responseModel.Data = CreateBranchResponse(branch, address);

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

            var branch = await _branchRepo.FindByCodeAsync(code);

            if (branch == null)
            {
                responseModel.AddError($"branch of code: {code} not found");
                return responseModel;
            }

            var address = await _addressRepo.FindByIdAsync(branch.AddressId);

            responseModel.Data = CreateBranchResponse(branch, address);

            return responseModel;
        }

        /// <summary>
        /// Add or Edit an existing branch 
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddOrEditBranchAsync(int branchId, CreateBranchRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            if (await BranchExists(request.Address.CountryId, request.Address.CityId, request.Name))
            {
                responseModel.AddError("branch name does already exist");
                return responseModel;
            }

            var user = _httpContextAccessor.HttpContext.User;

            if (branchId != 0)
            {
                var branch = await _branchRepo.FindByIdAsync(branchId);

                if (branch != null)
                {
                    branch.Name = request.Name;
                    branch.Code = request.Code;
                    branch.Address = request.Address;
                    branch.LastModifiedBy = user.Identity.Name;
                    branch.LastModifiedOn = DateTime.UtcNow;

                    await _branchRepo.UpdateAsync(_dbContext, branch);
                }
                else
                {
                    responseModel.AddError("branch not found");
                    return responseModel;
                }
            }
            else
            {
                var newAddress = CreateAddress(request);
                var newBranch = CreateBranch(request);

                if (newBranch == null)
                {
                    responseModel.AddError("couldn't create new branch");
                    return responseModel;
                }

                newBranch.CreatedBy = user.Identity.Name;

                var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

                using (dbContextTransaction)
                {
                    try
                    {
                        await _addressRepo.AddAsync(newAddress, _dbContext);

                        newBranch.AddressId = newAddress.Id;

                        await _branchRepo.AddAsync(_dbContext, newBranch);

                        await dbContextTransaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await dbContextTransaction.RollbackAsync();
                        responseModel.AddError(ex.ToString());
                    }
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
        public async Task<ApiResponse> DeleteBranchAsync(int branchId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            var branch = await _branchRepo.FindByIdAsync(branchId);

            if (branch == null)
            {
                responseModel.AddError($"branch {branchId} not found");
                return responseModel;
            }

            try
            {
                await _branchRepo.RemoveAsync(branch.Id);
            }
            catch (Exception ex)
            {
                responseModel.AddError(ex.ToString());
            }

            return responseModel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="branchName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BranchExists(int countryId, int cityId, string branchName)
        {
            return await _branchRepo.ExistsAsync(countryId, cityId, branchName);
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

        private Address CreateAddress(CreateBranchRequest request)
        {
            if (request != null)
            {
                var address = request.Address;

                return new Address()
                {
                    Name = address.Name,
                    DistrictId = address.DistrictId,
                    CityId = address.CityId,
                    CountryId = address.CountryId,
                    Street = address?.Street,
                    PostalCode = address?.PostalCode
                };
            }

            return null;
        }

        private BranchResponse CreateBranchResponse(Branch branch, Address address)
        {
            if (branch != null)
            {
                var branchAddress = CreateAddressResponse(address);
                return new BranchResponse(branch.Id, branch.Name, branch.Code, branch.Phone, branchAddress);
            }

            return null;
        }

        private AddressResponse CreateAddressResponse(Address address)
        {
            if (address != null)
            {
                return new AddressResponse(address.Id, address.Name,
                                           address.DistrictId, address.District.Name,
                                           address.CityId, address.City.Name,
                                           address.CountryId, address.Country.Name,
                                           address.Street, address.PostalCode);
            }

            return null;
        }

        #endregion
    }
}
