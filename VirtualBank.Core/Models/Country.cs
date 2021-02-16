using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Models
{
    public class Country
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
