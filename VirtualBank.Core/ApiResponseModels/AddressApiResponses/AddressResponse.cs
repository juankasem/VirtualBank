using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.AddressApiResponses
{
    public class AddressResponse
    {
        public string Street { get; set; }

        public int DistrictId { get; set; }

        public int CityId { get; set; }

        public int CountryId { get; set; }


        public AddressResponse(int districtId, int cityId, int countryId, string street)
        {
            DistrictId = Throw.ArgumentNullException.IfNull(districtId, nameof(districtId));
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Street = street;
        }
    }
}
