﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;
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
        [HttpGet("get/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCashTransactionsByAccountNoAsync([FromRoute] string accountNo, [FromQuery] int lastDays,
                                                                             CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);
            var customer = GetCustomerAsync(accountNo);


            if ( customer ==  null || user.Id != customer?.Id)
            {
                return Unauthorized();
            }

            try
            {
               return Ok(await _cashTransactionsService.GetCashTransactionsByAccountNoAsync(accountNo, lastDays, cancellationToken));
                
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // POST api/values
        [HttpPost]
        [Route("post")]
        public async Task<IActionResult> PostCashTransactionAsync([FromBody] CreateCashTransactionRequest request,
                                                                  CancellationToken cancellationToken = default)
        {
            try
            {
                await _cashTransactionsService.AddCashTransactionAsync(request, cancellationToken);

                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest();
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
