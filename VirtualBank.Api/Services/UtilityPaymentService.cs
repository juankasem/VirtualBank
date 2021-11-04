using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.UtilityPaymentApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.UtilityPaymentApiResponses;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class UtilityPaymentService : IUtilityPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UtilityPaymentService(IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ApiResponse<UtilityPaymentListResponse>> GetAllUtilityPaymentsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<UtilityPaymentListResponse>();

            var allUtilityPayments = await _unitOfWork.UtilityPayments.ListAllAsync(pageNumber, pageSize);

            if (!allUtilityPayments.Any())
            {
                return responseModel;
            }

            responseModel.Data = new(allUtilityPayments.ToImmutableList(), allUtilityPayments.Count());

            return responseModel;
        }


        public async Task<ApiResponse<UtilityPaymentListResponse>> GetUtilityPaymentsByIBANAsync(string iban, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<UtilityPaymentListResponse>();

            var ibanUtilityPayments = await _unitOfWork.UtilityPayments.GetByIBANAsync(iban, pageNumber, pageSize);

            if (!ibanUtilityPayments.Any())
            {
                return responseModel;
            }

            responseModel.Data = new(ibanUtilityPayments.ToImmutableList(), ibanUtilityPayments.Count());

            return responseModel;
        }


        public async Task<ApiResponse<UtilityPaymentResponse>> GetUtilityPaymentByIdsync(Guid utilityPaymentId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<UtilityPaymentResponse>();

            var utilityPayment = await _unitOfWork.UtilityPayments.FindByIdAsync(utilityPaymentId);

            if (utilityPayment == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(utilityPayment), $"utilityPayment of Id: {utilityPaymentId} not found"));

                return responseModel;
            }

            responseModel.Data = new(utilityPayment);

            return responseModel;
        }


        public async Task<Response> AddOrEditUtilityPaymentAsync(Guid utilityPaymentId, CreateUtilityPaymentRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new Response();

            if (utilityPaymentId != null)
            {
                var utilityPayment = await _unitOfWork.UtilityPayments.FindByIdAsync(utilityPaymentId);

                if (utilityPayment != null)
                {
                    utilityPayment.IBAN = request.IBAN;
                    utilityPayment.Code = request.Code;
                    utilityPayment.Address = request.Address;
                    utilityPayment.LastModifiedBy = request.CreationInfo.CreatedBy;
                    utilityPayment.LastModifiedOn = request.CreationInfo.CreatedOn;

                    try
                    {
                        await _unitOfWork.Branches.UpdateAsync(branch);
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
    }
}