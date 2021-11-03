using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.ActionResults;
using VirtualBank.Api.Cache;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class CashTransactionsController : ControllerBase
    {
        private readonly ICashTransactionsService _cashTransactionsService;
        private readonly ICustomerService _customerService;
        private readonly ICreditCardsService _creditCardsService;
        private readonly IDebitCardsService _debitCardsService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionResultMapper<CashTransactionsController> _actionResultMapper;

        public CashTransactionsController(ICashTransactionsService cashTransactionsService,
                                          ICustomerService customerService,
                                          ICreditCardsService creditCardsService,
                                          IDebitCardsService debitCardsService,
                                          UserManager<AppUser> userManager,
                                          IActionResultMapper<CashTransactionsController> actionResultMapper)
        {
            _cashTransactionsService = cashTransactionsService;
            _customerService = customerService;
            _creditCardsService = creditCardsService;
            _debitCardsService = debitCardsService;
            _userManager = userManager;
            _actionResultMapper = actionResultMapper;
        }


        // GET api/v1/cash-transactions/all?pageNumber=1&pageSize=50
        [Authorize(Roles = "Admin")]
        [Cached(600)]
        [HttpGet(ApiRoutes.CashTransactions.GetAll)]
        [ProducesResponseType(typeof(PagedResponse<CashTransactionListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllCashTransactions([FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                                [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                                CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _cashTransactionsService.GetAllCashTransactionsAsync(pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<CashTransactionListResponse>(apiResponse.Data);
                    return Ok(pagedApiResponse);
                }


                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET api/v1/cash-transactions/iban/TR123
        [HttpGet(ApiRoutes.CashTransactions.GetByIBAN)]
        [Cached(600)]
        [ProducesResponseType(typeof(PagedResponse<CashTransactionListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBankAccountCashTransactions([FromRoute] string iban,
                                                                        [FromQuery] int lastDays,
                                                                        [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                                        [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                                        CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = new ApiResponse<CashTransactionListResponse>();

                var user = await _userManager.GetUserAsync(User);
                var customer = await _customerService.GetCustomerByIBANAsync(iban, cancellationToken);

                if (customer == null)
                {
                    apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer)));
                    return NotFound(apiResponse);
                }

                if (user.Id != customer?.Data?.Customer?.UserId)
                {
                    apiResponse.AddError(ExceptionCreator.CreateForbiddenError(nameof(user), "user is not authorized to perform operation"));
                    return Forbid();
                }

                apiResponse = await _cashTransactionsService.GetBankAccountCashTransactionsAsync(iban, lastDays, pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<CashTransactionListResponse>(apiResponse.Data);

                    return Ok(pagedApiResponse);
                }

                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }



        // GET api/v1/cash-transactions/last-transactions/iban/TR123
        [HttpGet(ApiRoutes.CashTransactions.GetLatestTransfersByIBAN)]
        [Cached(600)]
        [ProducesResponseType(typeof(CashTransactionListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBankAccountLatestTransfers([FromRoute] string iban,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = new ApiResponse<LatestTransferListResponse>();

                var user = await _userManager.GetUserAsync(User);
                var customer = await _customerService.GetCustomerByIBANAsync(iban, cancellationToken);

                if (customer == null)
                {
                    apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer)));
                    return NotFound(apiResponse);
                }

                if (user.Id != customer?.Data?.Customer?.UserId)
                {
                    apiResponse.AddError(ExceptionCreator.CreateBadRequestError(nameof(user), "user is not authorized to perform operation"));
                    return BadRequest(apiResponse);
                }

                apiResponse = await _cashTransactionsService.GetBankAccountLatestTransfersAsync(iban, cancellationToken);

                if (apiResponse.Success)
                {
                    return Ok(apiResponse);
                }

                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // POST api/v1/cash-transactions
        [HttpPost(ApiRoutes.CashTransactions.Post)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddCashTransaction([FromBody] CreateCashTransactionRequest request,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = new Response();

                var user = await _userManager.GetUserAsync(User);
                var customer = await _customerService.GetCustomerByIBANAsync(request.From, cancellationToken);

                if (customer == null)
                {
                    apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer)));
                    return NotFound(apiResponse);
                }

                if (user.Id != customer.Data?.Customer?.UserId)
                {
                    apiResponse.AddError(ExceptionCreator.CreateBadRequestError(nameof(user), "user is not authorized to complete transaction"));
                    return BadRequest(apiResponse);
                }

                switch (request.Type)
                {
                    case CashTransactionType.Deposit:
                        apiResponse = await _cashTransactionsService.MakeDepositAsync(request, cancellationToken);
                        break;

                    case CashTransactionType.Withdrawal:
                        apiResponse = await _cashTransactionsService.MakeWithdrawalAsync(request, cancellationToken);
                        break;

                    case CashTransactionType.Transfer:
                        apiResponse = await _cashTransactionsService.MakeTransferAsync(request, cancellationToken);
                        break;

                    case CashTransactionType.EFT:
                        apiResponse = await _cashTransactionsService.MakeEFTAsync(request, cancellationToken);
                        break;
                }

                if (apiResponse.Success)
                    return Ok(apiResponse);

                if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status422UnprocessableEntity)
                    return UnprocessableEntity(apiResponse);


                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // POST api/v1/cash-transactions/credit-card
        [HttpPost(ApiRoutes.CashTransactions.Post)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddCashTransactionWithCreditOrDebitCard([FromBody] CreateCashTransactionRequest request,
                                                                                 CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = new Response();

                if (!string.IsNullOrEmpty(request.CreditCardNo))
                {
                    var creditCard = await _creditCardsService.GetCreditCardByAccountNoAsync(request.CreditCardNo, cancellationToken);

                    if (creditCard.Data == null)
                    {
                        apiResponse.AddError(ExceptionCreator.CreateNotFoundError($"Credit card No: {request.CreditCardNo} not found"));
                        return NotFound(apiResponse);
                    }

                    if (request.PIN == null)
                    {
                        apiResponse.AddError(ExceptionCreator.CreateBadRequestError("PIN is not found "));
                        return BadRequest(apiResponse);
                    }

                    if (!await _creditCardsService.ValidateCreditCardPINAsync(request.CreditCardNo, request.PIN))
                    {
                        apiResponse.AddError(ExceptionCreator.CreateBadRequestError($"Invalid PIN for Debit card of no: {request.DebitCardNo}"));
                        return BadRequest(apiResponse);
                    }
                }

                else if (!string.IsNullOrEmpty(request.DebitCardNo))
                {
                    var debitCard = await _debitCardsService.GetDebitCardByDebitCardNoAsync(request.DebitCardNo, cancellationToken);

                    if (debitCard.Data == null)
                    {
                        apiResponse.AddError(ExceptionCreator.CreateNotFoundError($"Debit card No: {request.DebitCardNo} not found"));
                        return NotFound(apiResponse);
                    }

                    if (request.PIN == null)
                    {
                        apiResponse.AddError(ExceptionCreator.CreateBadRequestError("PIN is not found "));
                        return BadRequest(apiResponse);
                    }

                    if (!await _debitCardsService.ValidateDebitCardPINAsync(request.DebitCardNo, request.PIN, cancellationToken))
                    {
                        apiResponse.AddError(ExceptionCreator.CreateBadRequestError($"Invalid PIN for Debit card of no: {request.DebitCardNo}"));
                        return BadRequest(apiResponse);
                    }
                }

                switch (request.Type)
                {
                    case CashTransactionType.Deposit:
                        apiResponse = await _cashTransactionsService.MakeDepositAsync(request, cancellationToken);
                        break;

                    case CashTransactionType.Withdrawal:
                        apiResponse = await _cashTransactionsService.MakeWithdrawalAsync(request, cancellationToken);
                        break;

                    case CashTransactionType.Transfer:
                        apiResponse = await _cashTransactionsService.MakeTransferAsync(request, cancellationToken);
                        break;

                    case CashTransactionType.EFT:
                        apiResponse = await _cashTransactionsService.MakeEFTAsync(request, cancellationToken);
                        break;
                }


                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status422UnprocessableEntity)
                    return UnprocessableEntity(apiResponse);


                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }
    }
}
