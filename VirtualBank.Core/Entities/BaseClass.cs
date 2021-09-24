using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    public class BaseClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public bool Disabled { get; set; } = false;

        public DateTime? DisabledOn { get; set; } = null;

        public string DisabledBy { get; set; } = null;
    }
}
