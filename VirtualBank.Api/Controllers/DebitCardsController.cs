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
using VirtualBank.Core.ApiRequestModels.DebitCardApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.DebitCardApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{


    [ApiController]
    [Authorize(Roles = "Adminisgtrator")]
    public class DebitCardsController : ControllerBase
    {

        private readonly IDebitCardsService _debitCardsService;
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionResultMapper<DebitCardsController> _actionResultMapper;

        public DebitCardsController(IDebitCardsService debitCardsService,
                                    ICustomerService customerService,
                                    UserManager<AppUser> userManager,
                                    IActionResultMapper<DebitCardsController> actionResultMapper)
        {
            _debitCardsService = debitCardsService;
            _customerService = customerService;
            _userManager = userManager;
            _actionResultMapper = actionResultMapper;
        }


        // GET: api/v1/debit-cards/all
        [Authorize(Roles = "Admin")]
        [HttpGet(ApiRoutes.DebitCards.GetAll)]
        [ProducesResponseType(typeof(PagedResponse<DebitCardListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllDebitCards([FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                           [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                           CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _debitCardsService.GetAllDebitCardsAsync(pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<DebitCardListResponse>(apiResponse.Data);

                    return Ok(pagedApiResponse);
                }


                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }

        // GET: api/v1/debit-cards/5
        [HttpGet(ApiRoutes.DebitCards.GetById)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDebitCardById([FromRoute] int debitCardId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _debitCardsService.GetDebitCardByIdAsync(debitCardId, cancellationToken);

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


        // GET: api/v1/debit-cards/5
        [HttpGet(ApiRoutes.DebitCards.GetById)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDebitCardByDebitCardNo([FromRoute] string debitCardNo, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _debitCardsService.GetDebitCardByDebitCardNoAsync(debitCardNo, cancellationToken);

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


        // GET: api/v1/debit-cards/account/5
        [HttpGet(ApiRoutes.DebitCards.GetByAccountNo)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDebitCardByAccountNo([FromRoute] string accountNo, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByAccountNoAsync(accountNo, cancellationToken);

            var apiResponse = new Response();

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
                apiResponse = await _debitCardsService.GetDebitCardByAccountNoAsync(accountNo, cancellationToken);

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


        // PUT api/v1/debit-cards/account/5
        [HttpPut(ApiRoutes.DebitCards.Post)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditDebitCard([FromRoute] int debitCardId, [FromBody] CreateDebitCardRequest request,
                                                              CancellationToken cancellationToken = default)
        {
            var apiResponse = new Response();

            var user = await _userManager.GetUserAsync(User);
            var customer = await _customerService.GetCustomerByCreditCardIdsync(debitCardId, cancellationToken);


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
                apiResponse = await _debitCardsService.AddOrEditDebitCardAsync(debitCardId, request, cancellationToken);

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


        // PUT api/v1/debit-cards/activate/5
        [Authorize(Roles = "Admin")]
        [HttpPut(ApiRoutes.DebitCards.Activate)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ActivateDebitCard([FromRoute] int debitCardId,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _debitCardsService.ActivateDebitCardAsync(debitCardId, cancellationToken);

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


        // PUT api/v1/debit-cards/deactivate/5
        [Authorize(Roles = "Admin")]
        [HttpPut(ApiRoutes.DebitCards.Deactivate)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeactivateDebitCard([FromRoute] int debitCardId,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _debitCardsService.DeactivateDebitCardAsync(debitCardId, cancellationToken);

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
