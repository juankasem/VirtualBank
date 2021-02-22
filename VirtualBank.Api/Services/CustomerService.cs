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
    public class CustomerService : ICustomerService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public CustomerService(VirtualBankDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer != null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }


            responseModel.Data = new CustomerResponse(customer);

            return responseModel;
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
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

        public async Task<ApiResponse> CreateOrUpdateCustomerAsync(string customerId, CreateCustomerRequest request,
                                                                   CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse();

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(a => a.Id == customerId);
            var existingCustomer = request.Customer;

            try
            {
                if (customer != null)
                {
                    customer.IdentificationNo = existingCustomer.IdentificationNo;
                    customer.IdentificationType = existingCustomer.IdentificationType;
                    customer.FirstName = existingCustomer.FirstName;
                    customer.MiddleName = existingCustomer.MiddleName.Length > 0 ? existingCustomer.MiddleName : "";
                    customer.LastName = existingCustomer.LastName;
                    customer.Gender = existingCustomer.Gender;
                    customer.Nationality = existingCustomer.Nationality;
                    customer.Address = existingCustomer.Address;
                    customer.BirthDate = existingCustomer.BirthDate;
                    customer.ModifiedOn = DateTime.UtcNow;
                }
                else
                {
                    var newCustomer = CreateCustomer(request);

                    if (newCustomer == null)
                    {
                        responseModel.AddError("couldn't create new account");
                        return responseModel;
                    }

                    await _dbContext.Customers.AddAsync(newCustomer);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                responseModel.AddError(ex.ToString());
            }

            return responseModel;
        }

        public async Task<ApiResponse> DeactivateCustomerAsync(string customerId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        #region Helper methods

        private Customer CreateCustomer(CreateCustomerRequest request)
        {
            var customer = request.Customer;

            if(customer != null)
            {
                var newCustomer = new Customer()
                {
                    IdentificationNo = customer.IdentificationNo,
                    IdentificationType = customer.IdentificationType,
                    FirstName = customer.FirstName,
                    MiddleName = customer.MiddleName,
                    LastName = customer.LastName,
                    FatherName = customer.FatherName,
                    Gender = customer.Gender,
                    Nationality = customer.Nationality,
                    BirthDate = customer.BirthDate,
                    Address = customer.Address
                };

                return newCustomer;
            }

            return null;     
        }

        #endregion
    }
}
