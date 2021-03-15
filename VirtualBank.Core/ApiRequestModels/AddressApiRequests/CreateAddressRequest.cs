using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.AddressApiRequests
{
    public class CreateAddressRequest
    {
        [MaxLength(150)]
        public string Street { get; set; }

        [Required]
        public int DistrictId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public int CountryId { get; set; }

       
        public CreateAddressRequest( int districtId, int cityId, int countryId, string street)
        {
            DistrictId = Throw.ArgumentNullException.IfNull(districtId, nameof(districtId));
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Street = street;
        }
    }
}
