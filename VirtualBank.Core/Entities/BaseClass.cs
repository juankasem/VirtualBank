using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    [NotMapped]
    public class BaseClass
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public AppUser CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } 

        public AppUser ModifiedBy { get; set; }

        public bool Disabled { get; set; } = false;

        public DateTime? DisabledOn { get; set; } = null;

        public AppUser DisabledBy { get; set; } = null;

    }
}
