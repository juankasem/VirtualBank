using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Models
{
    [NotMapped]
    public class Address
    {

        public string Street { get; set; }

        public int DistrictId { get; set; }

        public int CityId { get; set; }

        public int CountryId { get; set; }
    }
}
