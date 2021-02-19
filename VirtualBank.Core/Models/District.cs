using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Models
{
    public class District : BaseData
    {
        [Required]
        public string Name { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }
        public City City { get; set; }
    }
}
