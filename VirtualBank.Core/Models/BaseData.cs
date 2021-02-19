using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Models
{
    [NotMapped]
    public class BaseData
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public AppUser CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; } 

        public AppUser ModifieddBy { get; set; }

        public bool Disabled { get; set; } = false;

        public DateTime? DisabledOn { get; set; } = null;

        public AppUser DisabledBy { get; set; } = null;

    }
}
