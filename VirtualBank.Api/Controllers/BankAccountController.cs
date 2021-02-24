using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    /// <summary>
    /// Manage bank accounts
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;

        public BankAccountController(IBankAccountService bankAccountService,
                                     ICustomerService customerService,
                                     UserManager<AppUser> userManager)
        {
            _bankAccountService = bankAccountService;
            _customerService = customerService;
            _userManager = userManager;
        }

        // GET api/values/5
        [HttpGet("getByCustomerId/{customerId}")]
        public async Task<IActionResult> GetAccountsByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);
            var customer = GetCustomerByIdAsync(customerId);

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
                var apiResponse = await _bankAccountService.GetAccountsByCustomerIdAsync(customerId, cancellationToken);

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


        [HttpGet("getByAccountNo/{accountNo}")]
        public async Task<IActionResult> GetAccountsByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);
            var customer = GetCustomerByAccountNoAsync(accountNo);

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
                var apiResponse = await _bankAccountService.GetAccountByAccountNoAsync(accountNo, cancellationToken);

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

        [NonAction]
        private async Task<Customer> GetCustomerByIdAsync(string customerId)
        {
            var accountResponse = await _customerService.GetCustomerByIdAsync(customerId);

            if (accountResponse == null || accountResponse?.Data == null)
            {
                return null;
            }

            var customer = accountResponse?.Data?.Customer;

            return customer;
        }

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

    }
}
