using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerService(VirtualBankDbContext dbContext,
                               IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<ApiResponse<CustomerListResponse>> GetAllCustomersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerListResponse>();
            var skip = (pageNumber - 1) * pageSize;

            var customers = await _dbContext.Customers.Skip(skip).Take(pageSize).AsNoTracking().ToListAsync();

            var customerList = new List<CustomerResponse>();

            foreach (var customer in customers)
            {
                var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == customer.Address.Id);
                customerList.Add(CreateCustomerResponse(customer, address));
            }

            responseModel.Data = new CustomerListResponse(customerList.ToImmutableList(), customerList.Count);

            return responseModel;
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId && c.Disabled == false);

            if (customer != null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }

            var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == customer.AddressId);

            responseModel.Data = CreateCustomerResponse(customer, address);

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

            var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == customer.AddressId);


            responseModel.Data = CreateCustomerResponse(customer, address);

            return responseModel;
        }

     
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var account = await _dbContext.BankAccounts.FirstOrDefaultAsync(a => a.IBAN == iban && a.Disabled == false);

            if (account == null)
            {
                responseModel.AddError($"IBAN {iban} not found");
                return responseModel;
            }

            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == account.CustomerId && c.Disabled == false);

            if (customer == null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }

            var address = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == customer.AddressId);

            responseModel.Data = CreateCustomerResponse(customer, address);

            return responseModel;
        }


        public async Task<ApiResponse<RecipientCustomerResponse>> GetRecipientCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default)
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


        public async Task<ApiResponse> AddOrEditCustomerAsync(int customerId, CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse();

            if (await CustomerExists(request))
            {
                responseModel.AddError("customer name does already exist");
                return responseModel;
            }
             
            try
            {
                var user = _httpContextAccessor.HttpContext.User;
                var existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId && c.Disabled == false);

                if (existingCustomer != null)
                {
                    existingCustomer.IdentificationNo = request.IdentificationNo;
                    existingCustomer.IdentificationType = request.IdentificationType;
                    existingCustomer.FirstName = request.FirstName;
                    existingCustomer.MiddleName = request.MiddleName;
                    existingCustomer.LastName = request.LastName;
                    existingCustomer.Gender = request.Gender;
                    existingCustomer.Nationality = request.Nationality;
                    existingCustomer.Address = request.Address;
                    existingCustomer.BirthDate = request.BirthDate;
                    existingCustomer.LastModifiedOn = DateTime.UtcNow;
                    existingCustomer.LastModifiedBy = user.Identity.Name;

                    _dbContext.Customers.Update(existingCustomer);

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var newAddress = CreateAddress(request);
                    var newCustomer = CreateCustomer(request);

                    if (newCustomer == null)
                    {
                        responseModel.AddError("couldn't create new account");
                        return responseModel;
                    }

                    newCustomer.CreatedBy = user.Identity.Name;

                    var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

                    using (dbContextTransaction)
                    {
                        try
                        {
                            await _dbContext.Addresses.AddAsync(newAddress);
                            await _dbContext.SaveChangesAsync();

                            newCustomer.AddressId = newAddress.Id;
                            _dbContext.Customers.Update(newCustomer);

                            await _dbContext.Customers.AddAsync(newCustomer);
                            await _dbContext.SaveChangesAsync();

                            await dbContextTransaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await dbContextTransaction.RollbackAsync();
                            responseModel.AddError(ex.ToString());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                responseModel.AddError(ex.ToString());
            }

            return responseModel;
        }

        public async Task<ApiResponse> ActivateCustomerAsync(int customerId, CancellationToken cancellationToken = default)
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
                responseModel.AddError(ex.ToString());
            }

            return responseModel;
        }

        public async Task<ApiResponse> DeactivateCustomerAsync(int customerId, CancellationToken cancellationToken)
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
                responseModel.AddError(ex.ToString());
            }

            return responseModel;
        }

         public async Task<bool> CustomerExists(CreateCustomerRequest request)
        {
            return await _dbContext.Customers.AnyAsync(c => c.FirstName == request.FirstName && c.LastName == request.LastName
                                                            && c.FatherName == request.FatherName);
        }

        #region Helper methods

        private Customer CreateCustomer(CreateCustomerRequest request)
        {
            if (request != null)
            {
                return new Customer()
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
                    Address = request.Address
                };
            }

            return null;     
        }

        private Address CreateAddress(CreateCustomerRequest request)
        {
            if (request != null)
            {
                return new Address()
                {
                    Street = request.Address?.Street,
                    DistrictId = request.Address.DistrictId,
                    CityId = request.Address.CityId,
                    CountryId = request.Address.CountryId
                };
            }

            return null;
        }


        private CustomerResponse CreateCustomerResponse(Customer customer, Address address)
        {
            if (customer != null)
            {
                string fullName = customer.FirstName + " " +
                                  customer.MiddleName != "" ? customer.MiddleName : ""
                                  + " " + customer.LastName;

                var customerAddress = CreateAddressResponse(address);

                return new CustomerResponse(customer.Id, fullName, customer.Nationality,
                                            customer.Gender, customer.BirthDate, customer.UserId, customerAddress);
            }

            return null;
        }


        private RecipientCustomerResponse CreateRecipientCustomerResponse(Customer customer)
        {
            return new RecipientCustomerResponse(customer.FirstName, customer.LastName);
        }

        private AddressResponse CreateAddressResponse(Address address)
        {
            if (address != null)
            {
                return new AddressResponse(address.Id, address.DistrictId, address.District.Name,
                                           address.CityId, address.City.Name,
                                           address.CountryId, address.Country.Name,
                                           address.Street, address.PostalCode);
            }

            return null;
        }

        #endregion
    }
}
