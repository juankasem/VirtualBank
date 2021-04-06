using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class FastTransactionsService : IFastTransactionsService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBankAccountService _bankAccountService;
        private readonly IBranchService _branchService;


        public FastTransactionsService(VirtualBankDbContext dbContext,
                                      IHttpContextAccessor httpContextAccessor,
                                      IBankAccountService bankAccountService,
                                      IBranchService branchService)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _bankAccountService = bankAccountService;
            _branchService = branchService;
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
            var skip = (pageNumber - 1) * pageSize;

            var fastTransactions = await _dbContext.FastTransactions.ToListAsync();


            if (fastTransactions.Count() == 0)
            {
                return responseModel;
            }

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
        public async Task<ApiResponse<FastTransactionListResponse>> GetAccountFastTransactionsAsync(int accountId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<FastTransactionListResponse>();
            var skip = (pageNumber - 1) * pageSize;

            var fastTransactions = await _dbContext.FastTransactions.Where(f => f.AccountId == accountId &&
                                                                                f.Disabled == false).ToListAsync();


            if (fastTransactions.Count() == 0)
            {
                return responseModel;
            }

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

            var transaction = await _dbContext.FastTransactions.FirstOrDefaultAsync(f => f.Id == id && f.Disabled == false);

            if (transaction == null)
            {
                responseModel.AddError("fast transaction not found");
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
                var fastTransaction = await _dbContext.FastTransactions.FirstOrDefaultAsync(f => f.Id == id && f.Disabled == false);

                if (fastTransaction != null)
                {
                    fastTransaction.AccountId = request.AccountId;
                    fastTransaction.BranchId = request.BranchId;
                    fastTransaction.RecipientName = request.RecipientName;
                    fastTransaction.IBAN = request.IBAN;
                    fastTransaction.LastModifiedBy = user.Identity.Name;
                }
                else
                {
                    responseModel.AddError("fast transaction  not found");
                    return responseModel;
                }
            }
            else
            {
                var newFastTransaction = CreateFastTransaction(request);

                if (newFastTransaction == null)
                {
                    responseModel.AddError("couldn't create fast transaction");
                    return responseModel;
                }

                newFastTransaction.CreatedBy = user.Identity.Name;

                try
                {
                    await _dbContext.FastTransactions.AddAsync(newFastTransaction);
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ex.ToString());
                }
            }

            await _dbContext.SaveChangesAsync();

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

            var fastTransaction = await _dbContext.FastTransactions.FirstOrDefaultAsync(f => f.Id == id && f.Disabled == false);

            if (fastTransaction == null)
            {
                responseModel.AddError("fast transaction  not found");
                return responseModel;
            }

            fastTransaction.Disabled = true;

            _dbContext.FastTransactions.Update(fastTransaction);

            await _dbContext.SaveChangesAsync();

            return responseModel;
        }


        #region private helper methods
        private async Task<FastTransactionResponse> CreateFastTransactionResponse(FastTransaction transaction)
        {
            if (transaction != null)
            {
                var branchResponse = await _branchService.GetBranchByIdAsync(transaction.BranchId);

                if (branchResponse == null)
                {
                    return null;
                }

                var branch = branchResponse.Data;

                if (branch == null)
                {
                    return null;
                }

                var bankAccountResponse = await _bankAccountService.GetAccountByIdAsync(transaction.AccountId);

                if (bankAccountResponse == null)
                {
                    return null;
                }

                var bankAccount = bankAccountResponse.Data;


                if (bankAccount  == null)
                {
                    return null;
                }

                return new FastTransactionResponse(transaction.Id, bankAccount.IBAN, branch.Name, transaction.RecipientName, transaction.IBAN);
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
                    IBAN = request.IBAN
                };
            }

            return null;
        }

        #endregion
    }
}
