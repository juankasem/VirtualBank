using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;

        public CustomerController(VirtualBankDbContext dbContext, ICustomerService customerService, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _customerService = customerService;
            _userManager = userManager;
        }

        // GET api/values/5
        [HttpGet(ApiRoutes.getCustomerById)]
        public async Task<IActionResult> GetCustomerByIdAsync([FromRoute] string customerId, CancellationToken cancellationToken = default)
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
                var apiResponse = await _customerService.GetCustomerByIdAsync(customerId, cancellationToken);

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


        [HttpGet(ApiRoutes.getCustomerByAccountNo)]
        public async Task<IActionResult> GetCustomerByAccountNoAsync([FromRoute] string accountNo, CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);

            try
            {
                var apiResponse = await _customerService.GetCustomerByAccountNoAsync(accountNo, cancellationToken);

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


        [HttpPut(ApiRoutes.postCustomer)]
        public async Task<ActionResult<ApiResponse>> PostCustomerAsync([FromRoute] string customerId,[FromBody] CreateCustomerRequest request,
                                                                                  CancellationToken cancellationToken = default)
        {
            var user = await _userManager.GetUserAsync(User);

            try
            {
                var apiResponse = await _customerService.CreateOrUpdateCustomerAsync(customerId, request, cancellationToken);

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
    }
}
