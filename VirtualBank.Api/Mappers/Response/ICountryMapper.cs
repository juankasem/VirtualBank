using System.Collections.Immutable;
using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface ICountryMapper
    {
        Country MapToResponseModel(Core.Entities.Country country, ImmutableList<Country.City> cities = null);

        Address.Country MapToAddressCountry(Core.Entities.Country country);
    }

    public class CountryMapper : ICountryMapper
    {
        public Country MapToResponseModel(Core.Entities.Country country, ImmutableList<Country.City> cities = null) =>
        new(
            country.Id,
            country.Name,
            country.Code,
            CreateCreationInfo(country.CreatedBy, country.CreatedOn),
            CreateModificationInfo(country.LastModifiedBy, country.LastModifiedOn),
            cities != null ? cities : ImmutableList<Country.City>.Empty
        );

        public Address.Country MapToAddressCountry(Core.Entities.Country country) =>
        new(
            country.Id,
            country.Name,
            country.Code
        );

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string lastModifiedBy, DateTime lastModifiedOn) => new(lastModifiedBy, lastModifiedOn);
    }
}