using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Entities
{
    public class Address : BaseClass
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [ForeignKey(nameof(District))]
        public int DistrictId { get; set; }
        public District District { get; set; }

        [ForeignKey(nameof(City))]
        public int CityId { get; set; }
        public City City { get; set; }

        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [MaxLength(50)]
        public string Street { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }


        public Address(string name, int districtId, int cityId, int countryId,
                       string street, string postalCode, string createdBy, DateTime createdOn,
                       string lastModifiedBy, DateTime lastModifiedOn)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            DistrictId = districtId;
            CityId = cityId;
            CountryId = countryId;
            Street = street;
            PostalCode = postalCode;
            CreatedBy = Throw.ArgumentNullException.IfNull(createdBy, nameof(createdBy));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            LastModifiedBy = Throw.ArgumentNullException.IfNull(lastModifiedBy, nameof(lastModifiedBy));
            LastModifiedOn = Throw.ArgumentNullException.IfNull(lastModifiedOn, nameof(lastModifiedOn));
        }
    }
}
