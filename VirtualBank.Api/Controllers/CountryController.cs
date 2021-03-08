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
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;
        private readonly UserManager<AppUser> _userManager;

        public CountryController(ICountryService countryService, UserManager<AppUser> userManager)
        {
            _countryService = countryService;
            _userManager = userManager;
        }

        // GET: /<controller>/
        [HttpGet(ApiRoutes.getAllCountries)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllCountries(CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _countryService.GetAllCountries(cancellationToken);

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

        // GET api/values/5
        [HttpGet(ApiRoutes.getCountryById)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCountryById(string countryId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _countryService.GetCountryById(countryId, cancellationToken);

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

        // POST api/values
        [HttpPut(ApiRoutes.postCountry)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostCountryAsync([FromRoute] string countryId, [FromBody] CreateCountryRequest request,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _countryService.AddOrEditCountry(countryId, request, cancellationToken);

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
