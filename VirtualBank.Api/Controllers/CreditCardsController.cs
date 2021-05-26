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
using VirtualBank.Core.ApiRequestModels.CreditCardApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CreditCardApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class CreditCardsController : ControllerBase
    {
        private readonly ICreditCardsService _creditCardsService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionResultMapper<CreditCardsController> _actionResultMapper;

        public CreditCardsController(ICreditCardsService creditCardsService,
                                    ICustomerService customerService,
                                    UserManager<AppUser> userManager,
                                    IActionResultMapper<CreditCardsController> actionResultMapper)
        {
            _creditCardsService = creditCardsService;
            _customerService = customerService;
            _userManager = userManager;
            _actionResultMapper = actionResultMapper;
        }


        // GET: api/v1/credit-cards/all
        [Authorize(Roles = "Admin")]
        [HttpGet(ApiRoutes.CreditCards.GetAll)]
        [ProducesResponseType(typeof(PagedResponse<CreditCardListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllCreditCards([FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                           [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                           CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _creditCardsService.GetAllCreditCardsAsync(pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<CreditCardListResponse>(apiResponse.Data);

                    return Ok(pagedApiResponse);
                }


                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET: api/v1/credit-cards/5
        [HttpGet(ApiRoutes.CreditCards.GetById)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCreditCardById([FromRoute] int creditCardId, CancellationToken cancellationToken = default)
        {
            try
            {
               var apiResponse = await _creditCardsService.GetCreditCardByIdAsync(creditCardId, cancellationToken);

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


        // GET: api/v1/credit-cards/account/5
        [HttpGet(ApiRoutes.CreditCards.GetByAccountNo)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCreditCardByAccountNo([FromRoute] string accountNo, CancellationToken cancellationToken = default)
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
                apiResponse = await _creditCardsService.GetCreditCardByAccountNoAsync(accountNo, cancellationToken);

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



        // PUT: api/v1/credit-cards/5
        [HttpPut(ApiRoutes.CreditCards.Post)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditCreditCard([FromRoute] int creditCardId, [FromBody] CreateCreditCardRequest request,
                                                              CancellationToken cancellationToken = default)
        {
            var apiResponse = new ApiResponse();

            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByCreditCardIdsync(creditCardId, cancellationToken);


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
                apiResponse = await _creditCardsService.AddOrEditCreditCardAsync(creditCardId, request, cancellationToken);

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


        // PUT: api/v1/credit-cards/activate/5
        [Authorize(Roles = "Admin")]
        [HttpPut(ApiRoutes.CreditCards.Activate)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ActivateCreditCard([FromRoute] int creditCardId,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _creditCardsService.ActivateCreditCardAsync(creditCardId, cancellationToken);

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


        // PUT: api/v1/credit-cards/deactivate/5
        [Authorize(Roles = "Admin")]
        [HttpPut(ApiRoutes.CreditCards.Deactivate)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeactivateCreditCard([FromRoute] int creditCardId,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _creditCardsService.DeactivateCreditCardAsync(creditCardId, cancellationToken);

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
