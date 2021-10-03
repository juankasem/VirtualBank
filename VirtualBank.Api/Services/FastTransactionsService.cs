using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class FastTransactionsService : IFastTransactionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFastTransactionsMapper _fastTransactionsMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FastTransactionsService(IUnitOfWork unitOfWork,
                                       IFastTransactionsMapper fastTransactionsMapper,
                                       IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _fastTransactionsMapper = fastTransactionsMapper;
            _httpContextAccessor = httpContextAccessor;

        }

        /// <summary>
        /// Retrieve all fast transactions
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<FastTransactionListResponse>> GetAllFastTransactionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<FastTransactionListResponse>();

            var allFastTransactions = await _unitOfWork.FastTransactions.GetAll();

            if (!allFastTransactions.Any())
            {
                return responseModel;
            }

            var fastTransactions = allFastTransactions.OrderByDescending(c => c.CreatedOn)
                                                      .Skip((pageNumber - 1) * pageSize)
                                                      .Take(pageSize)
                                                      .Select(x => _fastTransactionsMapper.MapToResponseModel(x))
                                                      .ToImmutableList();

            responseModel.Data = new(fastTransactions, fastTransactions.Count);

            return responseModel;
        }

        /// <summary>
        /// Retrieve fast transactions that are associated to the account
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<FastTransactionListResponse>> GetBankAccountFastTransactionsAsync(string iban, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<FastTransactionListResponse>();

            var accountFastTransactions = await _unitOfWork.FastTransactions.GetByIBAN(iban);

            if (!accountFastTransactions.Any())
            {
                return responseModel;
            }

            var fastTransactions = accountFastTransactions.OrderByDescending(c => c.CreatedOn)
                                                          .Skip((pageNumber - 1) * pageSize)
                                                          .Take(pageSize)
                                                          .Select(x => _fastTransactionsMapper.MapToResponseModel(x))
                                                          .ToImmutableList();

            responseModel.Data = new(fastTransactions, fastTransactions.Count);

            return responseModel;
        }

        /// <summary>
        /// Retrieve fast transaction by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<FastTransactionResponse>> GetFastTransactionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<FastTransactionResponse>();

            var fastTransaction = await _unitOfWork.FastTransactions.FindByIdAsync(id);

            if (fastTransaction == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(fastTransaction)));
                return responseModel;
            }

            responseModel.Data = new(_fastTransactionsMapper.MapToResponseModel(fastTransaction));

            return responseModel;
        }


        /// <summary>
        /// Add or edit existing fast transaction
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<FastTransactionResponse>> AddOrEditFastTransactionAsync(int id, CreateFastTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<FastTransactionResponse>();

            if (id != 0)
            {
                var fastTransaction = await _unitOfWork.FastTransactions.FindByIdAsync(id);

                if (fastTransaction != null)
                {
                    fastTransaction.BankAccountId = request.BankAccountId;
                    fastTransaction.RecipientName = request.RecipientName;
                    fastTransaction.RecipientIBAN = request.RecipientIBAN;
                    fastTransaction.LastModifiedBy = request.ModificationInfo.ModifiedBy;
                    fastTransaction.LastModifiedOn = request.ModificationInfo.LastModifiedOn;

                    try
                    {
                        var updatedFastTransaction = await _unitOfWork.FastTransactions.UpdateAsync(fastTransaction);
                        await _unitOfWork.SaveAsync();

                        responseModel.Data = new(_fastTransactionsMapper.MapToResponseModel(updatedFastTransaction));
                    }
                    catch (Exception ex)
                    {
                        responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                        return responseModel;
                    }
                }
                else
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(fastTransaction)));
            }
            else
            {
                try
                {
                    var createdFastTransaction = await _unitOfWork.FastTransactions.AddAsync(CreateFastTransaction(request));
                    await _unitOfWork.SaveAsync();

                    responseModel.Data = new(_fastTransactionsMapper.MapToResponseModel(createdFastTransaction));
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
        /// Delete a fast transaction
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> DeleteFastTransactionAsync(int id, CancellationToken cancellationToken = default)
        {
            var responseModel = new Response();

            try
            {
                var isRemoved = await _unitOfWork.FastTransactions.RemoveAsync(id);

                if (!isRemoved)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError("fastTransaction"));
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
            }

            return responseModel;
        }


        #region private helper methods

        private FastTransaction CreateFastTransaction(CreateFastTransactionRequest request) =>
            new()
            {
                BankAccountId = request.BankAccountId,
                RecipientName = request.RecipientName,
                RecipientIBAN = request.RecipientIBAN,
                CreatedBy = request.CreationInfo.CreatedBy,
                CreatedOn = request.CreationInfo.CreatedOn
            };

        #endregion
    }
}
