using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.CreditCardApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CreditCardApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CreditCardsService : ICreditCardsService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICreditCardsRepository _creditCardsRepo;

        public CreditCardsService(VirtualBankDbContext dbContext,
                                  IHttpContextAccessor httpContextAccessor,
                                  ICreditCardsRepository creditCardsRepo)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _creditCardsRepo = creditCardsRepo;
        }


        /// <summary>
        /// Retrieve all credit cards
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CreditCardListResponse>> GetAllCreditCardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CreditCardListResponse>();

            var allCreditCards = await _creditCardsRepo.GetAllAsync();

            if (allCreditCards.Count() == 0)
            {
                return responseModel;
            }

            var creditCards = allCreditCards.OrderByDescending(b => b.CreatedAt).Skip((pageNumber - 1) * pageSize)
                                                                                .Take(pageSize);

            var creditCardList = new List<CreditCardResponse>();


            foreach (var creditCard in creditCards)
            {
                creditCardList.Add(CreateCreditCardResponse(creditCard));
            }

            responseModel.Data = new CreditCardListResponse(creditCardList.ToImmutableList(), creditCardList.Count());

            return responseModel;
        }

       

        /// <summary>
        /// Retrieve credit card by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CreditCardResponse>> GetCreditCardByIdAsync(int creditCardId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CreditCardResponse>();

            var creditCard = await _creditCardsRepo.FindByIdAsync(creditCardId);

            if (creditCard == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(creditCard), $"credit card of id {creditCardId}: not found"));
                return responseModel;
            }


            responseModel.Data = CreateCreditCardResponse(creditCard);

            return responseModel;
        }

        /// <summary>
        /// Retrieve credit card by account number
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CreditCardResponse>> GetCreditCardByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CreditCardResponse>();

            var creditCard = await _creditCardsRepo.FindByAccountNoAsync(accountNo);

            if (creditCard == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(creditCard), $"credit card of account No: {accountNo} not found"));
                return responseModel;
            }

            responseModel.Data = CreateCreditCardResponse(creditCard);

            return responseModel;
        }


        /// <summary>
        /// Add or Edit an existing credit card
        /// </summary>
        /// <param name="creditCardId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse> AddOrEditCreditCardAsync(int creditCardId, CreateCreditCardRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();

            if (creditCardId != 0)
            {
                var creditCard = await _creditCardsRepo.FindByIdAsync(creditCardId);

                try
                {
                    if (creditCard != null)
                    {
                        creditCard.CreditCardNo = request.CreditCardNo;
                        creditCard.ExpirationDate = request.ExpirationDate;
                        creditCard.AccountId = request.AccountId;
                        creditCard.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                        creditCard.LastModifiedOn = DateTime.UtcNow;

                        await _creditCardsRepo.UpdateAsync(creditCard);
                    }
                    else
                    {
                        responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(creditCard), $"credit card of id: {creditCardId} not found"));
                        return responseModel;
                    }
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError());
                }
            }
            else
            {
                try
                {
                    await _creditCardsRepo.AddAsync(CreateCreditCard(request));
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError());
                }
            }

            return responseModel;
        }


        #region private helper methods
        private CreditCard CreateCreditCard(CreateCreditCardRequest request)
        {
            if (request != null)
            {
                return new CreditCard()
                {
                    CreditCardNo = request.CreditCardNo,
                    PIN = request.PIN,
                    ExpirationDate = request.ExpirationDate,
                    AccountId = request.AccountId,
                    CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
                };
            }

            return null;
        }


        private CreditCardResponse CreateCreditCardResponse(CreditCard creditCard)
        {
            if (creditCard != null)
            {
                return new CreditCardResponse(creditCard.Id, creditCard.CreditCardNo,
                                              creditCard.ExpirationDate, creditCard.AccountId);
            }

            return null;
        }
        #endregion
    }
}
