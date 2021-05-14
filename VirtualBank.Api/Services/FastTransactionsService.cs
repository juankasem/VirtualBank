using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class FastTransactionsService : IFastTransactionsService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBankAccountRepository _bankAccountRepo;
        private readonly IBranchRepository _branchRepo;
        private readonly IFastTransactionsRepository _fastTransactionsRepo;

        public FastTransactionsService(VirtualBankDbContext dbContext,
                                      IHttpContextAccessor httpContextAccessor,
                                      IBankAccountRepository bankAccountRepo,
                                      IBranchRepository branchRepo,
                                      IFastTransactionsRepository fastTransactionsRepo)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _bankAccountRepo = bankAccountRepo;
            _branchRepo = branchRepo;
            _fastTransactionsRepo = fastTransactionsRepo;
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

            var allFastTransactions = await _fastTransactionsRepo.GetAll();

            if (allFastTransactions.Count() == 0)
            {
                return responseModel;
            }

            var fastTransactions = allFastTransactions.OrderByDescending(c => c.CreatedAt).Skip((pageNumber - 1) * pageSize)
                                                                                          .Take(pageSize);

            var fastTransactionList = new List<FastTransactionResponse>();

            foreach (var transaction in fastTransactions)
            {
                fastTransactionList.Add(await CreateFastTransactionResponse(transaction));
            }

            responseModel.Data = new FastTransactionListResponse(fastTransactionList.ToImmutableList(), fastTransactionList.Count());

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

            var accountFastTransactions = await _fastTransactionsRepo.GetByIBAN(iban);

            if (accountFastTransactions.Count() == 0)
            {
                return responseModel;
            }

            var fastTransactions = accountFastTransactions.OrderByDescending(c => c.CreatedAt).Skip((pageNumber - 1) * pageSize)
                                                                                              .Take(pageSize);

            var fastTransactionList = new List<FastTransactionResponse>();

            foreach (var transaction in fastTransactions)
            {
                fastTransactionList.Add(await CreateFastTransactionResponse(transaction));
            }

            responseModel.Data = new FastTransactionListResponse(fastTransactionList.ToImmutableList(), fastTransactionList.Count());

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

            var transaction = await _fastTransactionsRepo.FindByIdAsync(id);

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
        public async Task<ApiResponse> AddOrEditFastTransactionAsync(int id, CreateFastTransactionRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            var user = _httpContextAccessor.HttpContext.User;

            if (id != 0)
            {
                var fastTransaction = await _fastTransactionsRepo.FindByIdAsync(id);

                if (fastTransaction != null)
                {
                    fastTransaction.AccountId = request.AccountId;
                    fastTransaction.BranchId = request.BranchId;
                    fastTransaction.RecipientName = request.RecipientName;
                    fastTransaction.RecipientIBAN = request.RecipientIBAN;
                    fastTransaction.LastModifiedBy = user.Identity.Name;

                    try
                    {
                        await _fastTransactionsRepo.UpdateAsync(fastTransaction);
                    }
                    catch 
                    {
                        responseModel.AddError(ExceptionCreator.CreateInternalServerError());
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
                var newFastTransaction = CreateFastTransaction(request);

                if (newFastTransaction == null)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(newFastTransaction)));
                    return responseModel;
                }

                newFastTransaction.CreatedBy = user.Identity.Name;

                try
                {
                    await _fastTransactionsRepo.AddAsync(newFastTransaction);
                }
                catch 
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError());
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
        public async Task<ApiResponse> DeleteFastTransactionAsync(int id, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            try
            {
                var isRemoved = await _fastTransactionsRepo.RemoveAsync(id);

                if (!isRemoved)
                {
                    responseModel.AddError(ExceptionCreator.CreateNotFoundError("fastTransaction"));
                    return responseModel;
                }
            }
            catch 
            {
                responseModel.AddError(ExceptionCreator.CreateInternalServerError());
            }

            return responseModel;
        }


        #region private helper methods
        private async Task<FastTransactionResponse> CreateFastTransactionResponse(FastTransaction transaction)
        {
            if (transaction != null)
            {
                var branch = await _branchRepo.FindByIdAsync(transaction.BranchId);

                if (branch == null)
                {
                    return null;
                }

                var bankAccount = await _bankAccountRepo.FindByIdAsync(transaction.AccountId);

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
            if (request != null)
            {
                return new FastTransaction()
                {
                    AccountId = request.AccountId,
                    BranchId = request.BranchId,
                    RecipientName = request.RecipientName,
                    RecipientIBAN = request.RecipientIBAN
                };
            }

            return null;
        }

        #endregion
    }
}
