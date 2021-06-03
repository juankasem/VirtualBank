using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }


        // GET: api/v1/countries/all
        [HttpGet(ApiRoutes.Countries.GetAll)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllCountries(CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _countriesService.GetAllCountriesAsync(cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }


        // GET: api/v1/countries/5
        [HttpGet(ApiRoutes.Countries.GetById)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCountryById([FromRoute] int countryId, [FromQuery] bool includeCities = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _countriesService.GetCountryByIdAsync(countryId, includeCities, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }


        // PUT: api/v1/countries/5
        [HttpPut(ApiRoutes.Countries.Post)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditCountry([FromRoute] int countryId, [FromBody] CreateCountryRequest request,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
               var apiResponse = await _countriesService.AddOrEditCountryAsync(countryId, request, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.ToString());
            }
        }
    }
}
