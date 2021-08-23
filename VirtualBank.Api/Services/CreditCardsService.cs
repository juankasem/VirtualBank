using System;
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
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CreditCardsService : ICreditCardsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public CreditCardsService(IHttpContextAccessor httpContextAccessor,
                                 IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Retrieve all credit cards
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CreditCardListResponse>> GetAllCreditCardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CreditCardListResponse>();

            var allCreditCards = await _unitOfWork.CreditCards.GetAllAsync();

            if (!allCreditCards.Any())
            {
                return responseModel;
            }

            var creditCardList= allCreditCards.OrderByDescending(c => c.CreatedOn).Skip((pageNumber - 1) * pageSize)
                                                                                  .Take(pageSize)
                                                                                  .Select(c => CreateCreditCardResponse(c))
                                                                                  .ToImmutableList();

            responseModel.Data = new CreditCardListResponse(creditCardList, creditCardList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve credit cards for the specified IBAN
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CreditCardListResponse>> GetCreditCardsByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CreditCardListResponse>();

            var creditCards = await _unitOfWork.CreditCards.GetByIBANAsync(iban);

            if (creditCards == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError( $"credit card of IBAN: {iban}: not found"));
                return responseModel;
            }

            if (!creditCards.Any())
            {
                return responseModel;
            }

            var creditCardList = creditCards.OrderByDescending(c => c.CreatedOn).Select(c => CreateCreditCardResponse(c)).ToImmutableList();

            responseModel.Data = new CreditCardListResponse(creditCardList, creditCardList.Count());

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

            var creditCard = await _unitOfWork.CreditCards.FindByIdAsync(creditCardId);

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

            var creditCard = await _unitOfWork.CreditCards.FindByAccountNoAsync(accountNo);

            if (creditCard == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(creditCard), $"credit card of account No: {accountNo} not found"));
                return responseModel;
            }

            responseModel.Data = CreateCreditCardResponse(creditCard);

            return responseModel;
        }


        /// <summary>
        /// Validate creit card PIN
        /// </summary>
        /// <param name="debitCardNo"></param>
        /// <param name="pin"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ValidateCreditCardPINAsync(string creditCardNo, string pin, CancellationToken cancellationToken = default)
        {
            var isValid = await _unitOfWork.CreditCards.ValidatePINAsync(creditCardNo, pin);

            return isValid;
        }


        /// <summary>
        /// Add or Edit an existing credit card
        /// </summary>
        /// <param name="creditCardId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CreditCardResponse>> AddOrEditCreditCardAsync(int creditCardId, CreateCreditCardRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CreditCardResponse>();

            if (creditCardId != 0)
            {
                var creditCard = await _unitOfWork.CreditCards.FindByIdAsync(creditCardId);

                try
                {
                    if (creditCard != null)
                    {
                        creditCard.CreditCardNo = request.CreditCardNo;
                        creditCard.ExpirationDate = request.ExpirationDate;
                        creditCard.BankAccountId = request.BankAccountId;
                        creditCard.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                        creditCard.LastModifiedOn = DateTime.UtcNow;

                        var updatedCreditCard = await _unitOfWork.CreditCards.UpdateAsync(creditCard);

                        responseModel.Data = CreateCreditCardResponse(updatedCreditCard);

                        await _unitOfWork.CompleteAsync();
                    }
                    else
                    {
                        responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(creditCard), $"credit card of id: {creditCardId} not found"));
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
                   var createdCreditCard = await _unitOfWork.CreditCards.AddAsync(CreateCreditCard(request));
                   responseModel.Data = CreateCreditCardResponse(createdCreditCard);

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
        /// Activate credit card
        /// </summary>
        /// <param name="creditCardId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> ActivateCreditCardAsync(int creditCardId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CreditCardResponse>();

            var creditCard = await _unitOfWork.CreditCards.FindByIdAsync(creditCardId);

            if (creditCard != null)
            {
                creditCard.Disabled = false;
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(creditCard), $"credit card id: {creditCard} not found"));
            }

            return responseModel;
        }


        /// <summary>
        /// Deactivate credit card
        /// </summary>
        /// <param name="creditCardId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> DeactivateCreditCardAsync(int creditCardId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CreditCardResponse>();

            var creditCard = await _unitOfWork.CreditCards.FindByIdAsync(creditCardId);

            if (creditCard != null)
            {
                creditCard.Disabled = true;
            }
            else
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(creditCard), $"credit card id: {creditCard} not found"));
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
                    BankAccountId = request.BankAccountId,
                    CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
                };
            }

            return null;
        }


        private static CreditCardResponse CreateCreditCardResponse(CreditCard creditCard)
        {
            if (creditCard != null)
            {
                var creditCardHolder = creditCard.BankAccount?.Owner?.FirstName + " " + creditCard.BankAccount?.Owner?.LastName;

                return new CreditCardResponse(creditCard.Id, creditCard.CreditCardNo, creditCardHolder,
                                              creditCard.BankAccount?.IBAN, creditCard.ExpirationDate,
                                              creditCard.CreatedOn, (DateTime) creditCard.LastModifiedOn);
            }

            return null;
        }


        #endregion
    }
}
