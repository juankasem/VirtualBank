using VirtualBank.Core.Domain.Models;
using VirtualBank.Api.Helpers.Methods;

namespace VirtualBank.Api.Mappers.Response
{
    public interface ICityMapper
    {
        City MapToResponseModel(VirtualBank.Core.Entities.City city);

        Address.City MapToAddressCity(VirtualBank.Core.Entities.City city);
    }

    public class CityMapper : ICityMapper
    {
        private readonly ICountryMapper _countryMapper;

        public CityMapper(ICountryMapper countryMapper)
        {
            _countryMapper = countryMapper;
        }

        public City MapToResponseModel(Core.Entities.City city) =>
        new(
            city.Id,
            city.Name,
            _countryMapper.MapToResponseModel(city.Country),
            Utils.CreateCreationInfo(city.CreatedBy, city.CreatedOn),
            Utils.CreateModificationInfo(city.LastModifiedBy, city.LastModifiedOn)
        );

        public Address.City MapToAddressCity(Core.Entities.City City) =>
        new(
            City.Id,
            City.Name,
            City.CountryId
        );
    }
}