using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.DistrictApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.DistrictApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{

    public class DistrictsService : IDistrictsService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IDistrictMapper _districtMapper;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public DistrictsService(IUnitOfWork unitOfWork,
                                IDistrictMapper districtMapper,
                                IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _districtMapper = districtMapper;
            _httpContextAccessor = httpContextAccessor;
        }


        /// <summary>
        /// Retrieve all districts
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<DistrictListResponse>> ListDistrictsAsync(int cityId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DistrictListResponse>();

            IEnumerable<District> districts;

            if (cityId > 0)
            {
                districts = await _unitOfWork.Districts.GetByCityIdAsync(cityId);
            }
            else
            {
                districts = await _unitOfWork.Districts.GetAllAsync();
            }

            if (!districts.Any())
                return responseModel;


            var districtList = districts.OrderBy(district => district.Name).Select(district => _districtMapper.MapToResponseModel(district))
                                                                           .ToImmutableList();


            responseModel.Data = new(districtList, districtList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve district by id
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<DistrictResponse>> GetDistrictByIdAsync(int districtId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DistrictResponse>();

            var district = await _unitOfWork.Districts.FindByIdAsync(districtId);

            if (district == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(district), $"district Id: {districtId} not found"));
                return responseModel;
            }

            responseModel.Data = new(_districtMapper.MapToResponseModel(district));

            return responseModel;
        }


        /// <summary>
        /// Add or Edit an existing district
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<DistrictResponse>> AddOrEditDistrictAsync(int districtId, CreateDistrictRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DistrictResponse>();

            if (await _unitOfWork.Districts.DistrictNameExists(request.CityId, request.Name))
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError("cdistrict name does already exist"));
                return responseModel;
            }

            if (districtId != 0)
            {
                var district = await _unitOfWork.Districts.FindByIdAsync(districtId);


                if (district != null)
                {
                    district.CityId = request.CityId;
                    district.Name = request.Name;
                    district.LastModifiedBy = request.ModificationInfo.ModifiedBy;
                    district.LastModifiedOn = request.ModificationInfo.LastModifiedOn;

                    try
                    {
                        var updatedDistrict = await _unitOfWork.Districts.UpdateAsync(district);
                        await _unitOfWork.SaveAsync();

                        responseModel.Data = new(_districtMapper.MapToResponseModel(updatedDistrict));
                    }
                    catch (Exception ex)
                    {
                        responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                        return responseModel;
                    }
                }
                else
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(district), $"district of Id: {districtId} not found"));
            }
            else
            {
                try
                {
                    var createdDistrict = await _unitOfWork.Districts.AddAsync(CreateDistrict(request));
                    await _unitOfWork.SaveAsync();

                    responseModel.Data = new(_districtMapper.MapToResponseModel(createdDistrict));
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
        /// Check if distirct exists
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public async Task<bool> DistrictExists(int districtId) => await _unitOfWork.Districts.DistrictExists(districtId);


        #region private helper methods
        private static District CreateDistrict(CreateDistrictRequest request)
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

        #endregion
    }
}
