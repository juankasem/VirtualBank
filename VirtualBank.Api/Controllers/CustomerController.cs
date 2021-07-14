using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.ActionResults;
using VirtualBank.Api.Cache;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionResultMapper<CustomerController> _actionResultMapper;

        public CustomerController(ICustomerService customerService,
                                  UserManager<AppUser> userManager,
                                  IActionResultMapper<CustomerController> actionResultMapper)
        {
            _customerService = customerService;
            _userManager = userManager;
            _actionResultMapper = actionResultMapper;
        }


        // GET api/v1/customers/all
        [HttpGet(ApiRoutes.Customers.GetAll)]
        [Cached(600)]
        [ProducesResponseType(typeof(PagedResponse<CustomerListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                         [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _customerService.GetAllCustomersAsync(pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<CustomerListResponse>(apiResponse.Data);
                    return Ok(pagedApiResponse);
                }


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }
        

        // GET api/v1/customers/search
        [HttpGet(ApiRoutes.Customers.Search)]
        [Cached(600)]
        [ProducesResponseType(typeof(PagedResponse<CustomerListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchCustomersByName([FromQuery] string searchTerm,
                                                               [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                               [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                               CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _customerService.SearchCustomersByNameAsync(searchTerm, pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<CustomerListResponse>(apiResponse.Data);
                    return Ok(pagedApiResponse);
                }

                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET api/v1/customers/5
        [HttpGet(ApiRoutes.Customers.GetById)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCustomerById([FromRoute] int customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _customerService.GetCustomerByIdAsync(customerId, cancellationToken);

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


        // GET api/v1/customers/account/5
        [HttpGet(ApiRoutes.Customers.GetByAccountNo)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCustomerByAccountNo([FromRoute] string accountNo, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _customerService.GetCustomerByAccountNoAsync(accountNo, cancellationToken);

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


        // GET api/v1/customers/iban/5
        [HttpGet(ApiRoutes.Customers.GetByIBAN)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCustomerByIBAN([FromRoute] string iban, CancellationToken cancellationToken = default)
        {
            var user = _userManager.GetUserAsync(User);

            try
            {
                var apiResponse = await _customerService.GetCustomerByIBANAsync(iban, cancellationToken);

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


        // PUT api/v1/customers/5
        [HttpPut(ApiRoutes.Customers.Post)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditCustomer([FromRoute] int customerId,[FromBody] CreateCustomerRequest request,
                                                                       CancellationToken cancellationToken = default)
        {
            try
            {
               var apiResponse = await _customerService.AddOrEditCustomerAsync(customerId, request, cancellationToken);

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
    }
}
