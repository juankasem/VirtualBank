using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;
using VirtualBank.Core.Models;
using VirtualBank.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public CustomerController(VirtualBankDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<ApiResponse>> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            var customer = new Customer()
            {
                IdentificationNo = request.IdentificationNo,
                IdentificationType = request.IdentificationType,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                FatherName = request.FatherName,
                Gender = request.Gender,
                Nationality = request.Nationality,
                BirthDate = request.BirthDate,
                Address = request.Address,
                User = user
            };

            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();

            return Ok(new ApiResponse());
        }
    }
}
