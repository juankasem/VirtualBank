using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.AddressApiRequests
{
    public class CreateAddressRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int DistrictId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public int CountryId { get; set; }

        [MaxLength(150)]
        public string Street { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }


        public CreateAddressRequest(string name, int districtId, int cityId, int countryId, string street, string postalCode)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            DistrictId = Throw.ArgumentNullException.IfNull(districtId, nameof(districtId));
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Street = street;
            PostalCode = postalCode;
        }
    }
}
