using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Api.Helpers.Methods;

namespace VirtualBank.Api.Mappers.Response
{
    public interface ICustomerMapper
    {
        Customer MapToResponseModel(VirtualBank.Core.Entities.Customer customer);
        RecipientCustomer MapToRecipientCustomerResponseModel(VirtualBank.Core.Entities.Customer customer);
    }

    public class CustomerMapper : ICustomerMapper
    {
        private readonly IAddressMapper _addressMapper;

        public CustomerMapper(IAddressMapper addressMapper)
        {
            _addressMapper = addressMapper;
        }
        public Customer MapToResponseModel(Core.Entities.Customer customer) =>
            new(customer.Id,
              !string.IsNullOrEmpty(customer.MiddleName) ? GetFullName(customer.FirstName, customer.LastName, customer.MiddleName)
              : GetFullName(customer.FirstName, customer.LastName),
              customer.Nationality,
              customer.Gender,
              customer.BirthDate,
              customer.UserId,
              _addressMapper.MapToResponseModel(customer.Address),
              Utils.CreateCreationInfo(customer.CreatedBy, customer.CreatedOn),
              Utils.CreateModificationInfo(customer.LastModifiedBy, customer.LastModifiedOn)
            );

        public RecipientCustomer MapToRecipientCustomerResponseModel(Core.Entities.Customer customer)
        {
            throw new NotImplementedException();
        }

        private static string GetFullName(string firstName, string lastName, string middleName = null) =>
          !string.IsNullOrEmpty(middleName) ?
              firstName + ' ' + middleName + ' ' + lastName
                : firstName + ' ' + lastName;
    }
}