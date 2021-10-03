using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IAddressMapper
    {
        Address MapToResponseModel(VirtualBank.Core.Entities.Address address);
    }

    public class AddressMapper : IAddressMapper
    {
        private readonly IDistrictMapper _districtMapper;
        private readonly ICityMapper _cityMapper;
        private readonly ICountryMapper _countryMapper;

        public AddressMapper(IDistrictMapper districtMapper, ICityMapper cityMapper, ICountryMapper countryMapper)
        {
            _districtMapper = districtMapper;
            _cityMapper = cityMapper;
            _countryMapper = countryMapper;
        }
        public Address MapToResponseModel(Core.Entities.Address address) =>
            new(address.Id,
                address.Name,
                address.Street,
                address.PostalCode,
                _districtMapper.MapToAddressDistrict(address.District),
                _cityMapper.MapToAddressCity(address.City),
                _countryMapper.MapToAddressCountry(address.Country),
                CreateCreationInfo(address.CreatedBy, address.CreatedOn),
                CreateModificationInfo(address.LastModifiedBy, address.LastModifiedOn));

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string lastModifiedBy, DateTime lastModifiedOn) => new(lastModifiedBy, lastModifiedOn);
    }
}