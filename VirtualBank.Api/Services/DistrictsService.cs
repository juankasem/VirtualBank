using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.DistrictApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.DistrictApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{

    public class DistrictsService : IDistrictsService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DistrictsService(VirtualBankDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
    

        public async Task<ApiResponse<DistrictListResponse>> GetAllDistrictsAsync(CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DistrictListResponse>();

            var districtList = await _dbContext.Districts.OrderBy(c => c.Name).ToListAsync();

            var districts = new List<DistrictResponse>();

            foreach (var district in districtList)
            {
                districts.Add(CreateDistrictResponse(district));
            }

            responseModel.Data = new DistrictListResponse(districts.ToImmutableList(), districts.Count);

            return responseModel;
        }

        public async Task<ApiResponse<DistrictListResponse>> GetDistrictsByCityIdAsync(int cityId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DistrictListResponse>();

            var districtList = await _dbContext.Districts.Where(c => c.CityId == cityId).OrderBy(c => c.Name).ToListAsync();

            var districts = new List<DistrictResponse>();

            foreach (var district in districtList)
            {
                districts.Add(CreateDistrictResponse(district));
            }

            responseModel.Data = new DistrictListResponse(districts.ToImmutableList(), districts.Count);

            return responseModel;
        }

        public async Task<ApiResponse<DistrictResponse>> GetDistrictByIdAsync(int districtId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DistrictResponse>();

            var district = await _dbContext.Districts.FirstOrDefaultAsync(c => c.Id == districtId);


            responseModel.Data = CreateDistrictResponse(district);

            return responseModel;
        }

        public async Task<ApiResponse> AddOrEditDistrictAsync(int districtId, CreateDistrictRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            var user = _httpContextAccessor.HttpContext.User;
            var district = await _dbContext.Districts.FirstOrDefaultAsync(c => c.Id == districtId);

            if (district != null)
            {
                district.CityId = request.CityId;
                district.Name = request.Name;
                district.LastModifiedBy = user.Identity.Name;
                district.LastModifiedOn = DateTime.UtcNow;

                 _dbContext.Districts.Update(district);
            }
            else
            {
                var newDistrict = CreateDistrict(request);

                if (newDistrict == null)
                {
                    responseModel.AddError("couldn't create new city");
                    return responseModel;
                }

                newDistrict.CreatedBy = user.Identity.Name;

                await _dbContext.Districts.AddAsync(newDistrict);
            }

            await _dbContext.SaveChangesAsync();

            return responseModel;
        }

        public async Task<bool> DistrictExists(int cityId)
        {
            return await _dbContext.Districts.AnyAsync(c => c.Id == cityId);
        }

        #region private helper methods
        private District CreateDistrict(CreateDistrictRequest request)
        {
            if (request != null)
            {
                return new District()
                {
                    CityId = request.CityId,
                    Name = request.Name,
                };
            }

            return null;
        }

        private DistrictResponse CreateDistrictResponse(District district)
        {
            if (district != null)
            {
                return new DistrictResponse(district.Id, district.CityId, district.Name);
            }

            return null;
        }

        #endregion
    }
}
