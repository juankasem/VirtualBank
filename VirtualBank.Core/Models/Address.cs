using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Models
{
    [NotMapped]
    public class Address
    {

        public string Street { get; set; }

        [ForeignKey("District")]
        public int DistrictId { get; set; }
        public District District { get; set; }


        [ForeignKey("City")]
        public int CityId { get; set; }
        public City City { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }

    }
}
