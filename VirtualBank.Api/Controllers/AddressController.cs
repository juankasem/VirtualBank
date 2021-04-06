using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // GET: api/Address/getAlAddresses
        [HttpGet(ApiRoutes.getAllAddresses)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllAddresses([FromQuery] int pageNumber = PagingConstants.DefaultPageNumber,
                                                        [FromQuery] int pageSize = PagingConstants.DefaultPageSize,
                                                         CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _addressService.GetAllAddressesAsync(pageNumber, pageSize, cancellationToken);

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


        [HttpGet(ApiRoutes.getAddressById)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAddresshById(int branchId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiResponse = await _addressService.GetBranchByIdAsync(branchId, cancellationToken);

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
