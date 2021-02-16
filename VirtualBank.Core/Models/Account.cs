using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models
{
    public class Account
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string AccountNo { get; set; }

        [Required]
        public string IBAN { get; set; }


        [Required]
        public AccountType Type { get; set; }

        [ForeignKey("Owner")]
        [Required]
        public string OwnerId { get; set; }

        public AppUser Owner { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public AppUser CreatedBy { get; set; }

        public DateTime ModifiedAt { get; set; }

        public bool IsActive { get; set; }

    }
}
