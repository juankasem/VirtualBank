using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CashTransactionsController : ControllerBase
    {
        private readonly ICashTransactionsService _cashTransactionsService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;

        public CashTransactionsController(ICashTransactionsService cashTransactionsService,
                                          ICustomerService customerService,
                                          UserManager<AppUser> userManager)
        {
            _cashTransactionsService = cashTransactionsService;
            _customerService = customerService;
            _userManager = userManager;
        }

        // GET api/values/5
        [HttpGet(ApiRoutes.getCashTransactionsByIBAN)]
        [ProducesResponseType(typeof(PagedResponse<CashTransactionListResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCashTransactionsByIBAN([FromRoute] string iban,
                                                                   [FromQuery] int lastDays,
                                                                   [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,                                                                                                                                                             
                                                                   [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                                   CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);
            var customer = GetCustomerByIBANAsync(iban);

            if (customer == null)
            {
                return NotFound();
            }

            if (user.Id != customer?.Id)
            {
                return Unauthorized();
            }

            try
            {
                var apiResponse = await _cashTransactionsService.GetCashTransactionsByIBANAsync(iban,
                                                                                                lastDays,
                                                                                                pageNumber,
                                                                                                pageSize,
                                                                                                cancellationToken);

                if (apiResponse.Success)
                {
                  var pagedResponse = new PagedResponse<CashTransactionListResponse>(apiResponse.Data);

                  return Ok(pagedResponse);
                }

                else if (apiResponse.Errors[0].Contains("unauthorized"))
                    return Unauthorized(apiResponse);


                return BadRequest(apiResponse);

            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        // POST api/values
        [HttpPost(ApiRoutes.postCashTransaction)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostCashTransaction([FromBody] CreateCashTransactionRequest request,
                                                              CancellationToken cancellationToken = default)
        {
            try
            {
               var apiResponse =  await _cashTransactionsService.AddCashTransactionAsync(request, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Contains("not found"))
                    return NotFound(apiResponse);

                else if (apiResponse.Errors[0].Contains("not enough balance"))
                    return UnprocessableEntity(apiResponse);

                else if (apiResponse.Errors[0].Contains("unauthorized"))
                    return Unauthorized(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        #region
        [NonAction]
        private async Task<Customer> GetCustomerByAccountNoAsync(string accountNo)
        {
            var accountResponse = await _customerService.GetCustomerByAccountNoAsync(accountNo);

            if (accountResponse == null || accountResponse?.Data == null)
            {
                return null;
            }

            var customer = accountResponse?.Data?.Customer;

            return customer;
        }

        [NonAction]
        private async Task<Customer> GetCustomerByIBANAsync(string iban)
        {
            var accountResponse = await _customerService.GetCustomerByIBANAsync(iban);

            if (accountResponse == null || accountResponse?.Data == null)
            {
                return null;
            }

            var customer = accountResponse?.Data?.Customer;

            return customer;
        }
        #endregion
    }
}
