using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.ActionResults;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.CityApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICountriesService _countriesService;
        private readonly ICitiesService _citiesService;
        private readonly IActionResultMapper<CitiesController> _actionResultMapper;

        public CitiesController(ICountriesService countriesService,
                                ICitiesService citiesService,
                                IActionResultMapper<CitiesController> actionResultMapper)
        {;
            _countriesService = countriesService;
            _citiesService = citiesService;
            _actionResultMapper = actionResultMapper;
        }


        // GET: api/v1/cities/all
        [HttpGet(ApiRoutes.Cities.GetAll)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllCities(CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _citiesService.GetAllCitiesAsync(cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        // GET: api/v1/cities/country/5
        [HttpGet(ApiRoutes.Cities.GetByCountryId)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCitiesByCountryId([FromRoute] int countryId, CancellationToken cancellationToken = default)
        {
            var apiResponse = new ApiResponse();

            try
            {
                if (! await _countriesService.CountryExists(countryId))
                {
                    apiResponse.AddError(ExceptionCreator.CreateNotFoundError("country"));

                    return NotFound(apiResponse);
                }

                 apiResponse = await _citiesService.GetCitiesByCountryIdAsync(countryId, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }

        // GET: api/v1/cities/5
        [HttpGet(ApiRoutes.Cities.GetById)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCityById([FromRoute] int cityId, [FromQuery] bool includeCities = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _citiesService.GetCityByIdAsync(cityId, includeCities, cancellationToken);

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

        // PUT api/v1/cities/5
        [HttpPut(ApiRoutes.Cities.Post)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddOrEditCity([FromRoute] int cityId, [FromBody] CreateCityRequest request,
                                                       CancellationToken cancellationToken = default)
        {
            try
            {
               var apiResponse = await _citiesService.AddOrEditCityAsync(cityId, request, cancellationToken);

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
