﻿using System;
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
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [Route("api/[controller]")]
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
        [HttpGet("getByAccountNo/{id}")]
        [ProducesResponseType(typeof(CashTransactionsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCashTransactionsByAccountNoAsync([FromRoute] string accountNo, [FromQuery] int lastDays,
                                                                             CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);
            var customer = GetCustomerAsync(accountNo);

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
                var apiResponse = await _cashTransactionsService.GetCashTransactionsByAccountNoAsync(accountNo, lastDays, cancellationToken);

                if(apiResponse.Success)
                  return Ok(apiResponse);


                else if (apiResponse.Errors[0].Contains("unauthorized"))
                    return Unauthorized(apiResponse);

                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);

            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        // POST api/values
        [HttpPost("post")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostCashTransactionAsync([FromBody] CreateCashTransactionRequest request,
                                                                  CancellationToken cancellationToken = default)
        {
            try
            {
               var apiResponse =  await _cashTransactionsService.AddCashTransactionAsync(request, cancellationToken);

                if (apiResponse.Success)
                return Ok(apiResponse);

                
                else if (apiResponse.Errors[0].Contains("not found"))
                     return BadRequest(apiResponse);

                else if (apiResponse.Errors[0].Contains("unauthorized"))
                    return Unauthorized(apiResponse);

                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);

            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }
        
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [NonAction]
        private async Task<Customer> GetCustomerAsync(string accountNo)
        {
            var accountResponse = await _customerService.GetCustomerByAccountNoAsync(accountNo);

            if (accountResponse == null || accountResponse?.Data == null)
            {
                return null;
            }

            var customer = accountResponse?.Data?.Customer;

            return customer;
        }
    }
}
