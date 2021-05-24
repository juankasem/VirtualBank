using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.ActionResults;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionResultMapper<CashTransactionsController> _actionResultMapper;

        public CashTransactionsController(ICashTransactionsService cashTransactionsService,
                                          ICustomerService customerService,
                                          UserManager<AppUser> userManager,
                                          IActionResultMapper<CashTransactionsController> actionResultMapper)
        {
            _cashTransactionsService = cashTransactionsService;
            _customerService = customerService;
            _userManager = userManager;
            _actionResultMapper = actionResultMapper;
        }


        // GET api/v1/cash-transactions/all
        [Authorize(Roles = "Admin")]
        [HttpGet(ApiRoutes.CashTransactions.GetAll)]
        [ProducesResponseType(typeof(PagedResponse<CashTransactionListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
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
        [ProducesResponseType(typeof(PagedResponse<CashTransactionListResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
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

                if (user.Id != customer?.Data?.UserId)
                {
                    apiResponse.AddError(ExceptionCreator.CreateBadRequestError(nameof(user), "user is not authorized to perform operation"));
                    return BadRequest(apiResponse);
                }

                apiResponse = await _cashTransactionsService.GetBankAccountCashTransactionsAsync(iban,lastDays, pageNumber, pageSize, cancellationToken);

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


        // POST api/v1/cash-transactions
        [HttpPost(ApiRoutes.CashTransactions.Post)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddCashTransaction([FromBody] CreateCashTransactionRequest request,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = new ApiResponse();

                var user = await _userManager.GetUserAsync(User);
                var customer = await _customerService.GetCustomerByIBANAsync(request.From, cancellationToken);

                if (customer == null)
                {
                    apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer)));
                    return NotFound(apiResponse);
                }

                if (user.Id != customer?.Data?.UserId)
                {
                    apiResponse.AddError(ExceptionCreator.CreateBadRequestError(nameof(user), "user is not authorized to excecute transaction"));
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
                        apiResponse = await _cashTransactionsService.MakeEFTTransferAsync(request, cancellationToken);
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
