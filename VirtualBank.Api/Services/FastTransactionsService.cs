using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;


        public FastTransactionsService(IHttpContextAccessor httpContextAccessor,
                                     IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;

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

            var fastTransactions = allFastTransactions.OrderByDescending(c => c.CreatedAt)
                                                      .Skip((pageNumber - 1) * pageSize)
                                                      .Take(pageSize)
                                                      .Select(x => CreateFastTransactionResponse(x))
                                                      .Select(t => t.Result)
                                                      .ToImmutableList();
           
                
            responseModel.Data = new FastTransactionListResponse(fastTransactions, fastTransactions.Count);

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


            var fastTransactions = accountFastTransactions.OrderByDescending(c => c.CreatedAt)
                                                          .Skip((pageNumber - 1) * pageSize)
                                                          .Take(pageSize)
                                                          .Select(x => CreateFastTransactionResponse(x))
                                                          .Select(t => t.Result)
                                                          .ToImmutableList();


            responseModel.Data = new FastTransactionListResponse(fastTransactions, fastTransactions.Count);

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

            var transaction = await _unitOfWork.FastTransactions.FindByIdAsync(id);

            if (transaction == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(transaction)));
                return responseModel;
            }

            responseModel.Data = await CreateFastTransactionResponse(transaction);

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
                    fastTransaction.AccountId = request.BankAccountId;
                    fastTransaction.RecipientName = request.RecipientName;
                    fastTransaction.RecipientIBAN = request.RecipientIBAN;
                    fastTransaction.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                    fastTransaction.LastModifiedOn = DateTime.UtcNow;

                    try
                    {
                        await _unitOfWork.FastTransactions.UpdateAsync(fastTransaction);

                        await _unitOfWork.CompleteAsync();
                    }
                    catch (Exception ex)
                    {
                        responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                    }
                }
                else
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(fastTransaction)));
                    return responseModel;
                }
            }
            else
            {
                try
                {
                   var createdFastTransaction = await _unitOfWork.FastTransactions.AddAsync(CreateFastTransaction(request));

                    responseModel.Data = await CreateFastTransactionResponse(createdFastTransaction);

                    await _unitOfWork.CompleteAsync();
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
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
        private async Task<FastTransactionResponse> CreateFastTransactionResponse(FastTransaction transaction)
        {
            if (transaction != null)
            {
                var branch = await _unitOfWork.Branches.FindByIdAsync(transaction.Account.BranchId);

                if (branch == null)
                {
                    return null;
                }

                var bankAccount = await _unitOfWork.BankAccounts.FindByIdAsync(transaction.AccountId);

                if (bankAccount  == null)
                {
                    return null;
                }

                return new FastTransactionResponse(transaction.Id, bankAccount.IBAN, branch.Name, transaction.RecipientName, transaction.RecipientIBAN);
            }

            return null;
        }


        private FastTransaction CreateFastTransaction(CreateFastTransactionRequest request)
        {  
            return new FastTransaction()
            {
                AccountId = request.BankAccountId,
                RecipientName = request.RecipientName,
                RecipientIBAN = request.RecipientIBAN,
                CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
            }; 
        }

        #endregion
    }
}
