using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.DebitCardApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.DebitCardApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class DebitCardsService : IDebitCardsService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDebitCardsRepository _debitCardsRepo;
        private readonly IBankAccountRepository _bankAccountRepo;


        public DebitCardsService(VirtualBankDbContext dbContext,
                                 IHttpContextAccessor httpContextAccessor,
                                 IDebitCardsRepository debitCardsRepo,
                                 IBankAccountRepository bankAccountRepo)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _debitCardsRepo = debitCardsRepo;
            _bankAccountRepo = bankAccountRepo;
        }


        /// <summary>
        /// Retrieve all debit cards
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<DebitCardListResponse>> GetAllDebitCardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DebitCardListResponse>();

            var allDebitCards = await _debitCardsRepo.GetAllAsync();

            if (!allDebitCards.Any())
            {
                return responseModel;
            }

            var debitCards = allDebitCards.OrderByDescending(b => b.CreatedAt).Skip((pageNumber - 1) * pageSize)
                                                                              .Take(pageSize);

            var debitCardList = new List<DebitCardResponse>();


            foreach (var debitCard in debitCards)
            {
                debitCardList.Add(CreateDebitCardResponse(debitCard));
            }

            responseModel.Data = new DebitCardListResponse(debitCardList.ToImmutableList(), debitCardList.Count());

            return responseModel;
        }


        /// <summary>
        /// Retrieve debit card by id
        /// </summary>
        /// <param name="debitCardId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<DebitCardResponse>> GetDebitCardByIdAsync(int debitCardId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DebitCardResponse>();

            var debitCard = await _debitCardsRepo.FindByIdAsync(debitCardId);

            if (debitCard == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(DebitCard), $"Debit card of id: {debitCardId}: is not found"));
                return responseModel;
            }

            responseModel.Data = CreateDebitCardResponse(debitCard);

            return responseModel;
        }

        /// <summary>
        /// Retrieve debit card by debit card no
        /// </summary>
        /// <param name="debitCardNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<DebitCardResponse>> GetDebitCardByDebitCardNoAsync(string debitCardNo, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DebitCardResponse>();

            var debitCard = await _debitCardsRepo.FindByDebitCardNoAsync(debitCardNo);

            if (debitCard == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(debitCard), $"Debit card of number: {debitCardNo} is not found"));
                return responseModel;
            }

            responseModel.Data = CreateDebitCardResponse(debitCard);

            return responseModel;
        }

        /// <summary>
        /// Retrieve debit card by account number
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<DebitCardResponse>> GetDebitCardByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DebitCardResponse>();

            var debitCard = await _debitCardsRepo.FindByAccountNoAsync(accountNo);

            if (debitCard == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(debitCard), $"Debit card of account No: {accountNo} not found"));
                return responseModel;
            }

            responseModel.Data = CreateDebitCardResponse(debitCard);

            return responseModel;
        }


        /// <summary>
        /// Validate debit card PIN
        /// </summary>
        /// <param name="debitCardNo"></param>
        /// <param name="pin"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ValidateDebitCardPINAsync(string debitCardNo, string pin, CancellationToken cancellationToken = default)
        {
            var isValid = await _debitCardsRepo.ValidatePINAsync(debitCardNo, pin);

            return isValid;
        }


        /// <summary>
        /// Add or Edit an existing debit card
        /// </summary>
        /// <param name="debitCardId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddOrEditDebitCardAsync(int debitCardId, CreateDebitCardRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            if (debitCardId != 0)
            {
                var debitCard = await _debitCardsRepo.FindByIdAsync(debitCardId);

                try
                {
                    if (debitCard != null)
                    {
                        debitCard.DebitCardNo = request.DebitCardNo;
                        debitCard.ExpirationDate = request.ExpirationDate;
                        debitCard.BankAccountId = request.BankAccountId;
                        debitCard.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                        debitCard.LastModifiedOn = DateTime.UtcNow;

                        await _debitCardsRepo.UpdateAsync(debitCard);
                    }
                    else
                    {
                        responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(debitCard), $"Debit card of id: {debitCardId} not found"));
                        return responseModel;
                    }
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                }
            }
            else
            {
                try
                {
                    await _debitCardsRepo.AddAsync(CreateDebitCard(request));
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                }
            }

            return responseModel;
        }



        /// <summary>
        /// Activate debit card
        /// </summary>
        /// <param name="creditCardId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> ActivateDebitCardAsync(int debitCardId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DebitCardResponse>();

            var debitCard = await _debitCardsRepo.FindByIdAsync(debitCardId);

            if (debitCard != null)
            {
                debitCard.Disabled = false;
            }
            else
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(debitCard), $"debit card id: {debitCard } not found"));
            }

            return responseModel;
        }



        /// <summary>
        /// Deactivate debit card
        /// </summary>
        /// <param name="creditCardId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> DeactivateDebitCardAsync(int debitCardId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<DebitCardResponse>();

            var debitCard = await _debitCardsRepo.FindByIdAsync(debitCardId);

            if (debitCard != null)
            {
                debitCard.Disabled = true;
            }
            else
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(debitCard), $"debit card id: {debitCard} not found"));
            }

            return responseModel;
        }


        #region private helper methods

        private DebitCard CreateDebitCard(CreateDebitCardRequest request)
        {
            return new DebitCard()
            {
                DebitCardNo = request.DebitCardNo,
                PIN = request.PIN,
                ExpirationDate = request.ExpirationDate,
                BankAccountId = request.BankAccountId,
                CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
            };
        }

        private DebitCardResponse CreateDebitCardResponse(DebitCard debitCard)
        {        
            return new DebitCardResponse(debitCard.Id, debitCard.DebitCardNo,
                                         debitCard.ExpirationDate, debitCard.BankAccount.IBAN);
        }

       
        #endregion
    }
}
