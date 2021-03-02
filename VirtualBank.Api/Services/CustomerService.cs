using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerService(VirtualBankDbContext dbContext,
                               UserManager<AppUser> userManager,
                               IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId && c.Disabled == false);

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

            var account = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.AccountNo == accountNo && a.Disabled == false);

            if(account == null)
            {
                responseModel.AddError($"Account Number {accountNo} not found");
                return responseModel;
            }

             var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == account.CustomerId && c.Disabled == false);

            if (customer == null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }


            responseModel.Data = new CustomerResponse(customer);

            return responseModel;
        }

        public async Task<ApiResponse<RecipientCustomerResponse>> GetCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<RecipientCustomerResponse>();

            var account = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.IBAN == iban && a.Disabled == false);

            if (account == null)
            {
                responseModel.AddError($"Account with IBAN: {iban} not found");
                return responseModel;
            }

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == account.CustomerId && c.Disabled == false);

            if (customer == null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }

            responseModel.Data = CreateRecipientCustomerResponse(customer);

            return responseModel;
        }


        public async Task<ApiResponse> AddOrEditCustomerAsync(string customerId, CreateCustomerRequest request,
                                                                   CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse();
            var user = _httpContextAccessor.HttpContext.User;

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId && c.Disabled == false);
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
                    customer.ModifiedBy = user.Identity.Name;
                }
                else
                {
                    var newCustomer = CreateCustomer(request);

                    if (newCustomer == null)
                    {
                        responseModel.AddError("couldn't create new account");
                        return responseModel;
                    }

                    newCustomer.CreatedBy = user.Identity.Name;

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

        public async Task<ApiResponse> ActivateCustomerAsync(string customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse();
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(a => a.Id == customerId);

            try
            {
                if (customer != null)
                {
                    customer.Disabled = false;
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                responseModel.AddError("customer not found");
            }

            return responseModel;
        }

        public async Task<ApiResponse> DeactivateCustomerAsync(string customerId, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse();
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(a => a.Id == customerId);

            try
            {
                if (customer != null)
                {
                    customer.Disabled = true;
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                responseModel.AddError("customer not found");
            }

            return responseModel;
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

        private RecipientCustomerResponse CreateRecipientCustomerResponse(Customer customer)
        {
            return new RecipientCustomerResponse(customer.FirstName, customer.LastName);
        }

        #endregion
    }
}
