using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    public class District : BaseClass
    {
        [Required]
        public string Name { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public City City { get; set; }
    }
}
