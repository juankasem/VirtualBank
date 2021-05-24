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
    [Authorize]
    [ApiController]
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

        // GET api/v1/bank-account/customer/5
        [HttpGet(ApiRoutes.BankAccounts.GetByCustomerId)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBankAccountsByCustomerId([FromRoute] int customerId,
                                                                     CancellationToken cancellationToken = default)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByIdAsync(customerId, cancellationToken);

            var apiResponse = new ApiResponse();

            if (customer == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer)));

                return NotFound(apiResponse);
            }

            if (user.Id != customer?.Data?.UserId)
            {
                apiResponse.AddError(ExceptionCreator.CreateBadRequestError(nameof(user), "user is not authorized to complete transaction"));

                return BadRequest(apiResponse);
            }

            try
            {
                apiResponse = await _bankAccountService.GetBankAccountsByCustomerIdAsync(customerId, cancellationToken);

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


        // GET api/v1/bank-account/5
        [HttpGet(ApiRoutes.BankAccounts.GetById)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBankAccountById([FromRoute] int accountId, CancellationToken cancellationToken = default)
        {
            try
            {
               var apiResponse = await _bankAccountService.GetBankAccountByIdAsync(accountId, cancellationToken);

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


        // GET api/v1/bank-account/account-no/123
        [HttpGet(ApiRoutes.BankAccounts.GetByAccountNo)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBankAccountByAccountNo([FromRoute] string accountNo, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByAccountNoAsync(accountNo, cancellationToken);

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
                apiResponse = await _bankAccountService.GetBankAccountByAccountNoAsync(accountNo, cancellationToken);

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

        // GET api/v1/bank-account/iban/TR123
        [HttpGet(ApiRoutes.BankAccounts.GetByIBAN)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAccountByIBAN([FromRoute] string iban, CancellationToken cancellationToken = default)
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
                apiResponse.AddError(ExceptionCreator.CreateBadRequestError(nameof(user), "user is not authorized to complete transaction"));

                return BadRequest(apiResponse);
            }

            try
            {
                apiResponse = await _bankAccountService.GetBankAccountByIBANAsync(iban, cancellationToken);

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


        // GET api/v1/bank-account/recipient-iban/TR123
        [HttpGet(ApiRoutes.BankAccounts.GetRecipientByIBAN)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRecipientAccountByIBAN([FromRoute] string iban, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _bankAccountService.GetRecipientBankAccountByIBANAsync(iban, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return BadRequest(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }

        // PUT api/v1/bank-account/5
        [Authorize(Roles ="Admin")]
        [HttpPut(ApiRoutes.BankAccounts.Post)]
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

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }

        // PUT api/v1/bank-account/activate/5
        [Authorize(Roles = "Admin")]
        [HttpPost(ApiRoutes.BankAccounts.Post)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ActivateBankAccount([FromRoute] int accountId,
                                                             [FromRoute] int customerId,
                                                             CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _bankAccountService.ActivateBankAccountAsync(accountId, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return BadRequest(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }

        // PUT api/v1/bank-account/deactivate/5
        [Authorize(Roles = "Admin")]
        [HttpPost(ApiRoutes.BankAccounts.Deactivate)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeactivateBankAccount([FromRoute] int accountId,
                                                               [FromRoute] int customerId,
                                                               CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _bankAccountService.DeactivateBankAccountAsync(accountId, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return BadRequest(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }
    }
}
