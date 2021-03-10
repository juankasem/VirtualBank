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
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    /// <summary>
    /// Manage bank accounts
    /// </summary>
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
        [HttpGet(ApiRoutes.getAccountsByCustomerId)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAccountsByCustomerId([FromRoute] int customerId, CancellationToken cancellationToken = default)
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


        [HttpGet(ApiRoutes.getAccountByAccountNo)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAccountByAccountNo([FromRoute] string accountNo, CancellationToken cancellationToken = default)
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


        [HttpGet(ApiRoutes.getAccountByIBAN)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAccountByIBAN([FromRoute] string iban, CancellationToken cancellationToken = default)
        {

            try
            {
                var apiResponse = await _bankAccountService.GetAccountByIBANAsync(iban, cancellationToken);

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


        [HttpGet(ApiRoutes.getRecipientAccountByIBAN)]
        public async Task<IActionResult> GetRecipientAccountByIBAN([FromRoute] string iban, CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);

            try
            {
                var apiResponse = await _bankAccountService.GetRecipientAccountByIBANAsync(iban, cancellationToken);

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

        // POST api/values
        [HttpPut(ApiRoutes.postBankAccount)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostBankAccount([FromRoute] int accountId, [FromBody] CreateBankAccountRequest request,
                                                                          CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _bankAccountService.AddOrEditBankAccountAsync(accountId, request, cancellationToken);

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

        [HttpPost(ApiRoutes.activateBankAccount)]
        public async Task<IActionResult> ActivateBankAccount([FromRoute] int accountId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _bankAccountService.ActivateBankAccountAsync(accountId, cancellationToken);

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

        [HttpPost(ApiRoutes.deactivateBankAccount)]
        public async Task<IActionResult> DeactivateBankAccount([FromRoute] int accountId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _bankAccountService.DeactivateBankAccountAsync(accountId, cancellationToken);

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


        #region private helper methods
        [NonAction]
        private async Task<Customer> GetCustomerByIdAsync(int customerId)
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

        #endregion
    }
}
