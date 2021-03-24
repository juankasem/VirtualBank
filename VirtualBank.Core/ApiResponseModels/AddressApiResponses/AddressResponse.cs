using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.AddressApiResponses
{
    public class AddressResponse
    {
        public int Id { get; }

        public int DistrictId { get; }

        public string DistrictName { get; }

        public int CityId { get;  }

        public string CityName { get; }

        public int CountryId { get;  }

        public string CountryName { get; }

        public string Street { get; }

        public string PostalCode { get; }

        public AddressResponse(int id, int districtId, string districtName, int cityId, string cityName,
                               int countryId, string countryName, string street, string postalCode)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            DistrictId = Throw.ArgumentNullException.IfNull(districtId, nameof(districtId));
            DistrictName = Throw.ArgumentNullException.IfNull(districtName, nameof(districtName));
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            CityName = Throw.ArgumentNullException.IfNull(cityName, nameof(cityName));
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            CountryName = Throw.ArgumentNullException.IfNull(countryName, nameof(countryName));
            Street = street;
            PostalCode = postalCode;
        }
    }
}
