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
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly ICustomerRepository _customerRepo;
        private readonly IAddressRepository _addressRepo;
        private readonly IBankAccountRepository _bankAccountRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerService(VirtualBankDbContext dbContext,
                               ICustomerRepository customerRepo,
                               IAddressRepository addressRepo,
                               IBankAccountRepository bankAccountRepo,
                               IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _customerRepo = customerRepo;
            _addressRepo = addressRepo;
            _bankAccountRepo = bankAccountRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieve all customers
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CustomerListResponse>> GetAllCustomersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerListResponse>();

            var allCustomers = await _customerRepo.GetAllAsync();

            if (allCustomers.Count() == 0)
            {
                return responseModel;
            }

            var customers = allCustomers.OrderByDescending(b => b.CreatedAt).Skip((pageNumber - 1) * pageSize)
                                                                            .Take(pageSize);

            var customerList = new List<CustomerResponse>();

            foreach (var customer in customers)
            {
                var address = await _addressRepo.FindByIdAsync(customer.AddressId);
                customerList.Add(CreateCustomerResponse(customer, address));
            }

            responseModel.Data = new CustomerListResponse(customerList.ToImmutableList(), customerList.Count);

            return responseModel;
        }

        /// <summary>
        /// etrieve customer by customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var customer = await _customerRepo.FindByIdAsync(customerId);

            if (customer != null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }

            var address = await _addressRepo.FindByIdAsync(customer.AddressId);

            responseModel.Data = CreateCustomerResponse(customer, address);

            return responseModel;
        }

     
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var bankAccount = await _bankAccountRepo.FindByAccountNoAsync(accountNo);

            if(bankAccount == null)
            {
                responseModel.AddError($"Account Number {accountNo} not found");
                return responseModel;
            }

            var customer = await _customerRepo.FindByAccountNoAsync(accountNo);

            if (customer == null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }

            var address = await _addressRepo.FindByIdAsync(customer.AddressId);


            responseModel.Data = CreateCustomerResponse(customer, address);

            return responseModel;
        }

     
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var bankAccount = await _bankAccountRepo.FindByIBANAsync(iban);

            if (bankAccount == null)
            {
                responseModel.AddError($"IBAN {iban} not found");
                return responseModel;
            }

            var customer = await _customerRepo.FindByAccountNoAsync(bankAccount.AccountNo);

            if (customer == null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }

            var address = await _addressRepo.FindByIdAsync(customer.AddressId);

            responseModel.Data = CreateCustomerResponse(customer, address);

            return responseModel;
        }


        public async Task<ApiResponse<CustomerResponse>> GetCustomerByCreditCardIdsync(int creditCardId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var customer = await _customerRepo.FindByCreditCardIdAsync(creditCardId);

            if (customer != null)
            {
                responseModel.AddError("Account holder not found");
                return responseModel;
            }

            var address = await _addressRepo.FindByIdAsync(customer.AddressId);

            responseModel.Data = CreateCustomerResponse(customer, address);

            return responseModel;
        }


        public async Task<ApiResponse<RecipientCustomerResponse>> GetRecipientCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<RecipientCustomerResponse>();

            var account = await _bankAccountRepo.FindByIBANAsync(iban);

            if (account == null)
            {
                responseModel.AddError($"Account with IBAN: {iban} not found");
                return responseModel;
            }

            var customer = await _customerRepo.FindByIBANAsync(iban);

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

            if (await CustomerExistsAsync(request))
            {
                responseModel.AddError("customer name does already exist");
                return responseModel;
            }
             
            try
            {
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
                    existingCustomer.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;

                    await _customerRepo.UpdateAsync(existingCustomer);

                }
                else
                {
                    var newCustomer = CreateCustomer(request);

                    if (newCustomer == null)
                    {
                        responseModel.AddError("couldn't create new account");
                        return responseModel;
                    }

                    var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();

                    using (dbContextTransaction)
                    {
                        try
                        {
                           var newAddress = await _addressRepo.AddAsync(CreateAddress(request), _dbContext);

                            newCustomer.AddressId = newAddress.Id;
                            await _customerRepo.AddAsync(_dbContext, newCustomer);

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
            var customer = await _customerRepo.FindByIdAsync(customerId);

            try
            {
                if (customer != null)
                {
                    customer.Disabled = false;
                    await _customerRepo.UpdateAsync(customer);
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
            var customer = await _customerRepo.FindByIdAsync(customerId);

            try
            {
                if (customer != null)
                {
                    customer.Disabled = true;
                    await _customerRepo.UpdateAsync(customer);
                }
            }
            catch (Exception ex)
            {
                responseModel.AddError(ex.ToString());
            }

            return responseModel;
        }

         public async Task<bool> CustomerExistsAsync(CreateCustomerRequest request)
        {
            return await _customerRepo.CustomerExistsAsync(CreateCustomer(request));
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
                    TaxNumber = request.TaxNumber,
                    BirthDate = request.BirthDate,
                    Address = request.Address,
                    CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
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
                return new AddressResponse(address.Id, address.Name, address.DistrictId, address.District.Name,
                                           address.CityId, address.City.Name,
                                           address.CountryId, address.Country.Name,
                                           address.Street, address.PostalCode);
            }

            return null;
        }

        #endregion
    }
}
