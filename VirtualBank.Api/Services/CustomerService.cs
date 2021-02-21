using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class CustomerService : ICustomersService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public CustomerService(VirtualBankDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }


        public async Task<ApiResponse<CustomerResponse>> GetCustomerByAccountNo(string accountNo, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNo == accountNo);

            if(account != null)
            {
                responseModel.AddError($"Account Number {accountNo} not found");
                return responseModel;
            }

             var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == account.CustomerId);

            if (customer != null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }


            responseModel.Data = new CustomerResponse(customer);

            return responseModel;
        }

        public Task<ApiResponse> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> DeactivateCustomer(string customerId)
        {
            throw new NotImplementedException();
        }

      
    }
}
