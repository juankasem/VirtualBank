using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

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

        [Required]
        public CreationInfo CreationInfo { get; set; }

        [Required]
        public ModificationInfo ModificationInfo { get; set; }


        public CreateAddressRequest(string name, int districtId, int cityId, int countryId, string street, string postalCode,
                                    CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            DistrictId = Throw.ArgumentNullException.IfNull(districtId, nameof(districtId));
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Street = street;
            PostalCode = postalCode;
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}
