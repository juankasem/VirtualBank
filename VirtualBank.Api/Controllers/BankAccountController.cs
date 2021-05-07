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
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
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
        private readonly IActionResultMapper<BankAccountController> _actionResultMapper;

        public BankAccountController(IBankAccountService bankAccountService,
                                     ICustomerService customerService,
                                     UserManager<AppUser> userManager,
                                     IActionResultMapper<BankAccountController> actionResultMapper)
        {
            _bankAccountService = bankAccountService;
            _customerService = customerService;
            _userManager = userManager;
            _actionResultMapper = actionResultMapper;
        }

        // GET api/values/5
        [HttpGet(ApiRoutes.getAccountsByCustomerId)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAccountsByCustomerId([FromRoute] int customerId,
                                                                 [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                                 [FromQuery] int pageSize = PagingConstants.DefaultPageSize, 
                                                                 CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByIdAsync(customerId, cancellationToken);

            if (customer == null)
            {
                return NotFound();
            }

            if (user.Id != customer?.Data?.Id)
            {
                return Unauthorized();
            }

            try
            {
                var apiResponse = await _bankAccountService.GetBankAccountsByCustomerIdAsync(customerId, cancellationToken);

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
                return _actionResultMapper.Map(exception);
            }
        }

        [HttpGet(ApiRoutes.getAccountById)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAccountById([FromRoute] int accountId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                var apiResponse = await _bankAccountService.GetBankAccountByIdAsync(accountId, cancellationToken);

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
            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByAccountNoAsync(accountNo, cancellationToken);

            if (customer == null)
            {
                return NotFound();
            }

            if (user.Id != customer?.Data?.UserId)
            {
                return Unauthorized();
            }

            try
            {
                var apiResponse = await _bankAccountService.GetBankAccountByAccountNoAsync(accountNo, cancellationToken);

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
                return _actionResultMapper.Map(exception);
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
                var apiResponse = await _bankAccountService.GetBankAccountByIBANAsync(iban, cancellationToken);

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
                return _actionResultMapper.Map(exception);
            }
        }


        [HttpGet(ApiRoutes.getRecipientAccountByIBAN)]
        public async Task<IActionResult> GetRecipientAccountByIBAN([FromRoute] string iban, CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);

            try
            {
                var apiResponse = await _bankAccountService.GetRecipientBankAccountByIBANAsync(iban, cancellationToken);

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
                return _actionResultMapper.Map(exception);
            }
        }

        // POST api/values
        [HttpPut(ApiRoutes.postBankAccount)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditBankAccount([FromRoute] int accountId, [FromBody] CreateBankAccountRequest request,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _bankAccountService.AddOrEditBankAccountAsync(accountId, request, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Contains("not found"))
                    return NotFound(apiResponse);

                else if (apiResponse.Errors[0].Contains("unauthorized"))
                    return Unauthorized(apiResponse);

                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);

            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
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
                return _actionResultMapper.Map(exception);
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
                return _actionResultMapper.Map(exception);
            }
        }
    }
}
