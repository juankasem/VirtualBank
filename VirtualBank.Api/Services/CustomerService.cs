using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;
using VirtualBank.Core.ApiResponseModels.CustomerApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerService(IUnitOfWork unitOfWork,
                               IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
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

            var allCustomers = await _unitOfWork.Customers.GetAllAsync();

            if (!allCustomers.Any())
            {
                return responseModel;
            }

            var customers = allCustomers.OrderByDescending(b => b.CreatedOn).Skip((pageNumber - 1) * pageSize)
                                                                            .Take(pageSize)
                                                                            .Select(customer => CreateCustomerResponse(customer))
                                                                            .ToImmutableList();

            responseModel.Data = new CustomerListResponse(customers, customers.Count);

            return responseModel;
        }

        /// <summary>
        /// Search customers by name
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CustomerListResponse>> SearchCustomersByNameAsync(string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerListResponse>();

            var searchResult = await _unitOfWork.Customers.SearchByNameAsync(searchTerm);

            if (!searchResult.Any())
            {
                return responseModel;
            }

            var customers = searchResult.OrderByDescending(b => b.CreatedOn).Skip((pageNumber - 1) * pageSize)
                                                                            .Take(pageSize)
                                                                            .Select(customer => CreateCustomerResponse(customer))
                                                                            .ToImmutableList();

            responseModel.Data = new CustomerListResponse(customers, customers.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve customer by customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var customer = await _unitOfWork.Customers.FindByIdAsync(customerId);

            if (customer != null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer), $"customer of id: {customerId} not found"));
                return responseModel;
            }

            responseModel.Data = CreateCustomerResponse(customer);

            return responseModel;
        }


        /// <summary>
        /// Retrieve customer by account number
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByAccountNoAsync(accountNo);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank Account of No: {accountNo} not found"));
                return responseModel;
            }

            var customer = await _unitOfWork.Customers.FindByAccountNoAsync(accountNo);

            if (customer == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer), $"bank Account's holder of account No: {accountNo} not found"));
                return responseModel;
            }

            responseModel.Data = CreateCustomerResponse(customer);

            return responseModel;
        }


        /// <summary>
        /// Retrieve customer by IBAN
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(iban);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank Account of IBAN: {iban} not found"));
                return responseModel;
            }

            var customer = await _unitOfWork.Customers.FindByIBANAsync(iban);

            if (customer == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer), $"bank Account's holder of IBAN: {iban} not found"));
                return responseModel;
            }

            responseModel.Data = CreateCustomerResponse(customer);

            return responseModel;
        }


        /// <summary>
        /// Retrieve customer by credit card id
        /// </summary>
        /// <param name="creditCardId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CustomerResponse>> GetCustomerByCreditCardIdsync(int creditCardId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            var customer = await _unitOfWork.Customers.FindByCreditCardIdAsync(creditCardId);

            if (customer != null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer), $"Holder of credit card id: {creditCardId} not found"));
                return responseModel;
            }

            responseModel.Data = CreateCustomerResponse(customer);

            return responseModel;
        }


        /// <summary>
        /// Retrieve recipient customer by credit card id
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<RecipientCustomerResponse>> GetRecipientCustomerByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<RecipientCustomerResponse>();

            var bankAccount = await _unitOfWork.BankAccounts.FindByIBANAsync(iban);

            if (bankAccount == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(bankAccount), $"bank Account of IBAN: {iban} not found"));
                return responseModel;
            }

            var customer = await _unitOfWork.Customers.FindByIBANAsync(iban);

            if (customer == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer), $"bank Account's holder of IBAN: {iban} not found"));
                return responseModel;
            }

            responseModel.Data = CreateRecipientCustomerResponse(customer);

            return responseModel;
        }


        /// <summary>
        /// Add or Edit an existing customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<CustomerResponse>> AddOrEditCustomerAsync(int customerId, CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            var responseModel = new ApiResponse<CustomerResponse>();

            if (await CustomerExistsAsync(request))
            {
                responseModel.AddError(ExceptionCreator.CreateBadRequestError("customer", "customer name does already exist"));
                return responseModel;
            }

            if (customerId != 0)
            {
                var customer = await _unitOfWork.Customers.FindByIdAsync(customerId);

                try
                {
                    if (customer != null)
                    {
                        customer.IdentificationNo = request.IdentificationNo;
                        customer.IdentificationType = request.IdentificationType;
                        customer.FirstName = request.FirstName;
                        customer.MiddleName = request.MiddleName;
                        customer.LastName = request.LastName;
                        customer.Gender = request.Gender;
                        customer.Nationality = request.Nationality;
                        customer.Address = request.Address;
                        customer.BirthDate = request.BirthDate;
                        customer.LastModifiedOn = DateTime.UtcNow;
                        customer.LastModifiedBy = _httpContextAccessor.HttpContext.User.Identity.Name;

                        await _unitOfWork.Customers.UpdateAsync(customer);
                        await _unitOfWork.SaveAsync();
                    }
                    else
                    {
                        responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(customer), $"customer of id: {customerId} not found"));
                        return responseModel;
                    }
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                }
            }
            else
            {
                try
                {
                    var createdCustomer = await _unitOfWork.Customers.AddAsync(CreateCustomer(request));
                    responseModel.Data = CreateCustomerResponse(createdCustomer);

                    await _unitOfWork.CompleteTransactionAsync();
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                }
            }

            return responseModel;
        }


        /// <summary>
        /// Activate customer of the specified id
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> ActivateCustomerAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new Response();
            var customer = await _unitOfWork.Customers.FindByIdAsync(customerId);

            try
            {
                if (customer != null)
                {
                    customer.Disabled = false;
                    await _unitOfWork.Customers.UpdateAsync(customer);

                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
            }

            return responseModel;
        }


        /// <summary>
        /// Deactivate customer of the specified id
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> DeactivateCustomerAsync(int customerId, CancellationToken cancellationToken)
        {
            var responseModel = new Response();
            var customer = await _unitOfWork.Customers.FindByIdAsync(customerId);

            try
            {
                if (customer != null)
                {
                    customer.Disabled = true;
                    await _unitOfWork.Customers.UpdateAsync(customer);

                    await _unitOfWork.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
            }

            return responseModel;
        }


        /// <summary>
        /// Check if customer exists
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> CustomerExistsAsync(CreateCustomerRequest request)
        {
            return await _unitOfWork.Customers.CustomerExistsAsync(CreateCustomer(request));
        }


        #region private Helper methods

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
                    CreatedBy = request.CreationInfo.CreatedBy,
                    CreatedOn = request.CreationInfo.CreatedOn
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


        private CustomerResponse CreateCustomerResponse(Customer customer)
        {
            if (customer != null)
            {
                string fullName = customer.FirstName + " " +
                                  customer.MiddleName != "" ? customer.MiddleName : ""
                                  + " " + customer.LastName;

                var customerAddress = CreateAddressResponse(customer.Address);

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
                return new AddressResponse(address.Id,
                                           address.Name,
                                           address.DistrictId,
                                           address.District.Name,
                                           address.CityId,
                                           address.City.Name,
                                           address.CountryId,
                                           address.Country.Name,
                                           address.Street,
                                           address.PostalCode);
            }

            return null;
        }

        #endregion
    }
}
