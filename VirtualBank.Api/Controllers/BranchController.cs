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
using VirtualBank.Core.ApiRequestModels.BranchApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.BranchApiResponses;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "admin")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;
        private readonly ICitiesService _citiesService;
        private readonly IActionResultMapper<BranchController> _actionResultMapper;

        public BranchController(IBranchService branchService,
                                ICitiesService citiesService,
                                ICustomerService customerService,
                                UserManager<AppUser> userManager,
                                IActionResultMapper<BranchController> actionResultMapper)
        {
            _branchService = branchService;
            _citiesService = citiesService;
            _actionResultMapper = actionResultMapper;
        }


        // GET: api/v1/branch/all
        [HttpGet(ApiRoutes.Branches.List)]
        [Cached(600)]
        [ProducesResponseType(typeof(PagedResponse<CustomerListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ListBranches([FromQuery] int countryId,
                                                      [FromQuery] int cityId,
                                                      [FromQuery] int districtId,
                                                      [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                      [FromQuery] int pageSize = PagingConstants.DefaultPageSize,       
                                                      CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _branchService.ListBranchesAsync(pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<BranchListResponse>(apiResponse.Data);
                    return Ok(pagedApiResponse);
                }


                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET: api/v1/branch/search
        [HttpGet(ApiRoutes.Branches.Search)]
        [Cached(600)]
        [ProducesResponseType(typeof(PagedResponse<CustomerListResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchBranchesByName([FromQuery] string searchTerm,
                                                              [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                              [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                              CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _branchService.SearchBranchesByNameAsync(searchTerm, pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                {
                    var pagedApiResponse = new PagedResponse<BranchListResponse>(apiResponse.Data);
                    return Ok(pagedApiResponse);
                }

                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET: api/v1/branch/city/5
        [HttpGet(ApiRoutes.Branches.GetByCityId)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBranchesByCityId([FromRoute] int cityId,
                                                             [FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                             [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                             CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = new Response();

                if (!await _citiesService.CityExists(cityId))
                {
                    return NotFound(apiResponse);
                }

                apiResponse = await _branchService.GetBranchesByCityIdAsync(cityId, pageNumber, pageSize, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);


                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET: api/v1/branch/5
        [HttpGet(ApiRoutes.Branches.GetById)]
        [Cached(600)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBranchById(int branchId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _branchService.GetBranchByIdAsync(branchId, cancellationToken);

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

        // GET: api/v1/branch/code/5
        [HttpGet(ApiRoutes.Branches.GetByCode)]
        [Cached(600)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBranchByCode(string code, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _branchService.GetBranchByCodeAsync(code, cancellationToken);

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


        // PUT api/v1/branch/5
        [HttpPut(ApiRoutes.Branches.Post)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditBranchAsync([FromRoute] int branchId, [FromBody] CreateBranchRequest request,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
               var apiResponse = await _branchService.AddOrEditBranchAsync(branchId, request, cancellationToken);

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


        // DELETE api/v1/branch/5
        [HttpDelete(ApiRoutes.Branches.Delete)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteBranchAsync([FromRoute] int branchId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _branchService.DeleteBranchAsync(branchId, cancellationToken);

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
