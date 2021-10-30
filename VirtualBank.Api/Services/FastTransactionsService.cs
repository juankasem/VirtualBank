using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Models;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class FastTransactionsService : IFastTransactionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFastTransactionsMapper _fastTransactionsMapper;

        public FastTransactionsService(IUnitOfWork unitOfWork,
                                       IFastTransactionsMapper fastTransactionsMapper)
        {
            _unitOfWork = unitOfWork;
            _fastTransactionsMapper = fastTransactionsMapper;
        }

        #region public service methods
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

            var fastTransactions = allFastTransactions.OrderByDescending(c => c.CreationInfo.CreatedOn)
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

            var fastTransactions = accountFastTransactions.OrderByDescending(c => c.CreationInfo.CreatedOn)
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

            var recipientBankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(request.RecipientIBAN);

            if (recipientBankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(recipientBankAccount)));

                return responseModel;
            }

            if (recipientBankAccount.Owner.FullName != request.RecipientFullName)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(recipientBankAccount), "Recipient Name not found"));

                return responseModel;
            }

            if (id != 0)
            {
                var fastTransaction = await _unitOfWork.FastTransactions.FindByIdAsync(id);

                if (fastTransaction != null)
                {
                    fastTransaction.RecipientDetails = CreateRecipientDetails(recipientBankAccount.Id, request.RecipientIBAN, recipientBankAccount.Branch.Name,
                                                                              request.RecipientFullName, request.RecipientShortName,
                                                                              CreateMoney(request.AmountToTransfer.Amount, request.AmountToTransfer.Currency));
                    fastTransaction.ModificationInfo = CreateModificationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn);

                    try
                    {
                        var updatedFastTransaction = await _unitOfWork.FastTransactions.UpdateAsync(fastTransaction);
                        await _unitOfWork.SaveAsync();

                        responseModel.Data = new(_fastTransactionsMapper.MapToResponseModel(updatedFastTransaction.ToDomainModel()));
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
                    var createdFastTransaction = await _unitOfWork.FastTransactions.AddAsync(CreateFastTransaction(request, recipientBankAccount));
                    await _unitOfWork.SaveAsync();

                    responseModel.Data = new(_fastTransactionsMapper.MapToResponseModel(createdFastTransaction.ToDomainModel()));
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
        #endregion

        #region private helper methods
        private FastTransaction CreateFastTransaction(CreateFastTransactionRequest request, BankAccount bankAccount) =>
            new(0,
                request.IBAN,
                CreateRecipientDetails(bankAccount.Id, request.RecipientIBAN, bankAccount.Branch.Name,
                                       request.RecipientFullName, request.RecipientShortName,
                                       CreateMoney(request.AmountToTransfer.Amount, request.AmountToTransfer.Currency)),
                CreateCreationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn),
                CreateModificationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn));


        private RecipientDetails CreateRecipientDetails(int bankAccountId, string iban, string bankName,
                                                        string recipientFullName, string recipientShortName,
                                                        Money amountToTransfer) =>

                new(bankAccountId, iban, bankName, recipientFullName, recipientShortName, amountToTransfer);


        private Core.Models.Money CreateMoney(decimal amount, string currency) =>
             new Core.Models.Money(new Amount(amount), currency);

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string modifiededBy, DateTime lastModifiedeOn) => new(modifiededBy, lastModifiedeOn);

        #endregion
    }
}
