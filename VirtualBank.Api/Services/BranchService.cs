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

        /// <summary>
        /// Retrieve all branches
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchListResponse>> GetAllBranchesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchListResponse>();
            var skip = (pageNumber - 1) * pageSize;

            var branches = await _dbContext.Branches.Skip(skip).AsNoTracking().ToListAsync();

            var branchList = new List<BranchResponse>();

            foreach (var branch in branches)
            {
                var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == branch.AddressId);
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
        public async Task<ApiResponse<BranchListResponse>> GetBranchesByCityIdAsync(int cityId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchListResponse>();

            var branches = await _dbContext.Branches.Include(b => b.Address.CityId == cityId).ToListAsync();

            var branchList = new List<BranchResponse>();

            foreach (var branch in branches)
            {
                var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == branch.AddressId);
                branchList.Add(CreateBranchResponse(branch, address));
            }

            responseModel.Data = new BranchListResponse(branchList.ToImmutableList(), branchList.Count);

            return responseModel;
        }

        /// <summary>
        /// Retrieve the branch by the specified id
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<BranchResponse>> GetBranchByIdAsync(int branchId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<BranchResponse>();

            var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == branchId);

            if (branch == null)
            {
                responseModel.AddError($"branch {branchId} not found");
                return responseModel;
            }

            var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == branch.AddressId);

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

            var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Code == code);

            if (branch == null)
            {
                responseModel.AddError($"branch of code: {code} not found");
                return responseModel;
            }

            var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == branch.AddressId);

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

            if (await BranchExists(request.Name))
            {
                responseModel.AddError("branch name does already exist");
                return responseModel;
            }

            var user = _httpContextAccessor.HttpContext.User;
            var branch = await _dbContext.Branches.FirstOrDefaultAsync(b => b.Id == branchId);
           
            if (branch != null)
            {
                branch.Name = request.Name;
                branch.Code = request.Code;
                branch.Address = request.Address;
                branch.LastModifiedBy = user.Identity.Name;
                branch.LastModifiedOn = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
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
                        await _dbContext.Addresses.AddAsync(newAddress);
                        await _dbContext.SaveChangesAsync();

                        newBranch.AddressId = newAddress.Id;
                        _dbContext.Branches.Update(newBranch);

                        await _dbContext.Branches.AddAsync(newBranch);
                        await _dbContext.SaveChangesAsync();

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
        /// 
        /// </summary>
        /// <param name="branchName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BranchExists(string branchName)
        {
            return await _dbContext.Branches.AnyAsync(b => b.Name == branchName);
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
                return new Address()
                {
                    Street = request.Address?.Street,
                    DistrictId = request.Address.DistrictId,
                    CityId = request.Address.CityId,
                    CountryId = request.Address.CountryId
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
                return new AddressResponse(address.Id, address.DistrictId, address.District.Name,
                                           address.CityId, address.City.Name,
                                           address.CountryId, address.Country.Name,
                                           address.Street, address.PostalCode);
            }

            return null;
        }

        #endregion
    }
}
