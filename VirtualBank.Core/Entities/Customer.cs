using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
    public class Customer : BaseClass
    {
        [Required]
        [MinLength(8)]
        public string IdentificationNo { get; set; }

        [Required]
        public IdentificationType IdentificationType { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string FatherName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nationality { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public Address Address { get; set; }

        [ForeignKey(nameof(User))]
        [MaxLength(450)]
        public string UserId { get; set; }

        public AppUser User { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; }

        public ICollection<CreditCard> CreditCards { get; set; }
    }
}
