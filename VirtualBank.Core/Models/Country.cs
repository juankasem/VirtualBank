using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Models
{
    public class Country : BaseData
    {
        [Required]
        public string Name { get; set; }
    }
}
