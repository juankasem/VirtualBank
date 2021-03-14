using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public DistrictsController(IDistrictsService districtsService, ICitiesService citiesService)
        {
            _districtsService = districtsService;
            _citiesService = citiesService;
        }


        // GET: /<controller>/
        [HttpGet(ApiRoutes.getAllDistricts)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllDistricts(CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _districtsService.GetAllDistrictsAsync(cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Contains("unauthorized"))
                    return Unauthorized(apiResponse);

                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }



        // GET: /<controller>/
        [HttpGet(ApiRoutes.getDistrictsByCityId)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDistrictsByCityId([FromRoute] int cityId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!await _citiesService.CityExists(cityId))
                {
                    return NotFound();
                }

                var apiResponse = await _districtsService.GetDistrictByCityIdAsync(cityId, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Contains("unauthorized"))
                    return Unauthorized(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }

        // POST api/values
        [HttpPut(ApiRoutes.postDistrict)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostDistrict([FromRoute] int districtId, [FromBody] CreateDistrictRequest request,
                                                   CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _districtsService.AddOrEditDistrictAsync(districtId, request, cancellationToken);

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
    }
}
