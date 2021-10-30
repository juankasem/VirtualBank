using VirtualBank.Core.Domain.Models;
using VirtualBank.Api.Helpers.Methods;

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
                Utils.CreateCreationInfo(address.CreatedBy, address.CreatedOn),
                Utils.CreateModificationInfo(address.LastModifiedBy, address.LastModifiedOn));
    }
}