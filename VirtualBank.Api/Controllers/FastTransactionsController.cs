using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
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


        public FastTransactionsController(IFastTransactionsService fastTransactionsService, IBankAccountService bankAccountService)
        {
            _fastTransactionsService = fastTransactionsService;
            _bankAccountService = bankAccountService;
        }


        // GET: /<controller>/
        [HttpGet(ApiRoutes.getAllFastTransactions)]
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

                else if (apiResponse.Errors[0].Contains("not found"))
                    return NotFound(apiResponse);

                else if (apiResponse.Errors[0].Contains("unauthorized"))
                    return Unauthorized(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        // GET: /<controller>/accountId
        [HttpGet(ApiRoutes.getAccountFastTransactions)]
        [ProducesResponseType(typeof(PagedResponse<FastTransactionListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAccountFastTransactions([FromRoute] int accountId,
                                                                    [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                                    [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                                    CancellationToken cancellationToken = default)
        {
            try
            {
                var bankAccount = await _bankAccountService.GetBankAccountByIdAsync(accountId);

                if (bankAccount.Data == null)
                {
                    return NotFound();
                }

                var apiResponse = await _fastTransactionsService.GetAccountFastTransactionsAsync(accountId, pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedResponse = new PagedResponse<FastTransactionListResponse>(apiResponse.Data);

                    return Ok(pagedResponse);
                }


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        // GET: /<controller>/id
        [HttpGet(ApiRoutes.getFastTransactionById)]
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
                else if (apiResponse.Errors[0].Contains("not found"))
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        // POST api/values
        [HttpPost(ApiRoutes.postFastTransaction)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditFastTransaction([FromRoute] int id,
                                                             [FromBody] CreateFastTransactionRequest request,
                                                              CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _fastTransactionsService.AddOrEditFastTransactionAsync(id, request, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Contains("not found"))
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        // POST api/values
        [HttpPost(ApiRoutes.deleteFastTransaction)]
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

                else if (apiResponse.Errors[0].Contains("not found"))
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
