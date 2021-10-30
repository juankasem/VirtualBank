using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.ActionResults;
using VirtualBank.Api.Cache;
using VirtualBank.Core.ApiRequestModels.LoanApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.LoanApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class LoansController : ControllerBase
    {
        private readonly ILoansService _loansService;
        private readonly IActionResultMapper<LoansController> _actionResultMapper;

        public LoansController(ILoansService loansService,
                               IActionResultMapper<LoansController> actionResultMapper)
        {
            _loansService = loansService;
            _actionResultMapper = actionResultMapper;
        }


        // GET: /<controller>/
        [Authorize(Roles = "Admin")]
        [Cached(600)]
        [HttpGet(ApiRoutes.Loans.GetAll)]
        [ProducesResponseType(typeof(PagedResponse<LoanListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllLoans([FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                     [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                     CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _loansService.GetAllLoansAsync(pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<LoanListResponse>(apiResponse.Data);
                    return Ok(pagedApiResponse);
                }


                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET: api/v1/loan/5
        [HttpGet(ApiRoutes.Loans.GetById)]
        [Cached(600)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLoanById([FromRoute] Guid loanId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _loansService.GetLoanByIdsync(loanId, cancellationToken);

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


        // GET: api/v1/loan/customer/5
        [HttpGet(ApiRoutes.Loans.GetByCustomerId)]
        [Cached(600)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLoansByCustomerId([FromRoute] int customerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _loansService.GetLoansByCustomerIdAsync(customerId, cancellationToken);

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


        // PUT api/v1/loan/5
        [HttpPut(ApiRoutes.Branches.Post)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditLoanAsync([FromRoute] Guid loanId, [FromBody] CreateLoanRequest request,
                                                            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _loansService.AddOrEditLoanAsync(loanId, request, cancellationToken);

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
