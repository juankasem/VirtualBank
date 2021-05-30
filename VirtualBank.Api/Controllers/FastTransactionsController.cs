using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.ActionResults;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class FastTransactionsController : ControllerBase
    {
        private readonly IFastTransactionsService _fastTransactionsService;
        private readonly IBankAccountService _bankAccountService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionResultMapper<FastTransactionsController> _actionResultMapper;

        public FastTransactionsController(IFastTransactionsService fastTransactionsService,
                                          IBankAccountService bankAccountService,
                                          ICustomerService customerService,
                                          UserManager<AppUser> userManager,
                                          IActionResultMapper<FastTransactionsController> actionResultMapper)
        {
            _fastTransactionsService = fastTransactionsService;
            _bankAccountService = bankAccountService;
            _customerService = customerService;
            _userManager = userManager;
            _actionResultMapper = actionResultMapper;
        }


        // GET: api/v1/cash-transactions/all
        [Authorize("Admin")]
        [HttpGet(ApiRoutes.FastTransactions.GetAll)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllFastTransactions([FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                                [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                                 CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _fastTransactionsService.GetAllFastTransactionsAsync(pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);



                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET: api/v1/cash-transactions/iban/TR5
        [HttpGet(ApiRoutes.FastTransactions.GetByIBAN)]
        [ProducesResponseType(typeof(PagedResponse<FastTransactionListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBankAccountFastTransactions([FromRoute] string iban,
                                                                        [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                                        [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                                        CancellationToken cancellationToken = default)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByIBANAsync(iban, cancellationToken);

            var apiResponse = new ApiResponse<FastTransactionListResponse>();

            if (customer == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer)));

                return NotFound(apiResponse);
            }

            if (user.Id != customer?.Data?.UserId)
            {
                apiResponse.AddError(ExceptionCreator.CreateBadRequestError(nameof(user), "user is not authorized to complete this operation"));

                return BadRequest(apiResponse);
            }

            try
            {
                var bankAccount = await _bankAccountService.GetBankAccountByIBANAsync(iban);

                if (bankAccount.Data == null)
                {
                    return NotFound();
                }

                apiResponse = await _fastTransactionsService.GetBankAccountFastTransactionsAsync(iban, pageNumber, pageSize, cancellationToken);

                if (!apiResponse.Success) return BadRequest(apiResponse);
                var pagedResponse = new PagedResponse<FastTransactionListResponse>(apiResponse.Data);

                return Ok(pagedResponse);


            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }


        // GET: api/v1/cash-transactions/5
        [HttpGet(ApiRoutes.FastTransactions.GetById)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFastTransactionById([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _fastTransactionsService.GetFastTransactionByIdAsync(id, cancellationToken);

                if (apiResponse.Success)
                {
                    return Ok(apiResponse);
                }
                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        // PUT api/v1/cash-transactions/5
        [HttpGet(ApiRoutes.FastTransactions.Post)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditFastTransaction([FromRoute] int id,
                                                                  [FromRoute] string iban,
                                                                  [FromBody] CreateFastTransactionRequest request,
                                                                  CancellationToken cancellationToken = default)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByIBANAsync(iban, cancellationToken);

            var apiResponse = new ApiResponse();

            if (customer == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer)));

                return NotFound(apiResponse);
            }

            if (user.Id != customer?.Data?.UserId)
            {
                apiResponse.AddError(ExceptionCreator.CreateBadRequestError(nameof(user), "user is not authorized to complete this operation"));

                return BadRequest(apiResponse);
            }

            try
            {
                apiResponse = await _fastTransactionsService.AddOrEditFastTransactionAsync(id, request, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }


        // DELETE /cash-transactions/5
        [HttpDelete(ApiRoutes.FastTransactions.Delete)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteFastTransaction([FromRoute] int id,
                                                                CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _fastTransactionsService.DeleteFastTransactionAsync(id, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }
    }
}
