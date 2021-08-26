using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
