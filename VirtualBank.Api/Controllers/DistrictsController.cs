using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.ActionResults;
using VirtualBank.Core.ApiRequestModels.DistrictApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    public class DistrictsController : Controller
    {
        private readonly IDistrictsService _districtsService;
        private readonly ICitiesService _citiesService;
        private readonly IActionResultMapper<DistrictsController> _actionResultMapper;

        public DistrictsController(IDistrictsService districtsService,
                                   ICitiesService citiesService,
                                   IActionResultMapper<DistrictsController> actionResultMapper)
        {
            _districtsService = districtsService;
            _citiesService = citiesService;
            _actionResultMapper = actionResultMapper;
        }


        // GET: /<controller>/
        [HttpGet(ApiRoutes.getAllDistricts)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllDistricts(CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _districtsService.GetAllDistrictsAsync(cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }



        // GET: /<controller>/
        [HttpGet(ApiRoutes.getDistrictsByCityId)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDistrictsByCityId([FromRoute] int cityId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!await _citiesService.CityExists(cityId))
                {
                    return NotFound();
                }

                var apiResponse = await _districtsService.GetDistrictsByCityIdAsync(cityId, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);



                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }

        // POST api/values
        [HttpPut(ApiRoutes.postDistrict)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditDistrict([FromRoute] int districtId, [FromBody] CreateDistrictRequest request,
                                                      CancellationToken cancellationToken = default)
        {
            var apiResponse = new ApiResponse();

            if (!ModelState.IsValid)
            {
                return BadRequest(apiResponse);
            }

            try
            {
                apiResponse = await _districtsService.AddOrEditDistrictAsync(districtId, request, cancellationToken);

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
