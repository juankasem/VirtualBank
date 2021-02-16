using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models
{
    public class Customer
    {
        [Required]
        [MinLength(8)]
        public string IdentificationNo { get; set; }

        [Required]
        public IdentificationType IdentificationType { get; set; }


        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FatherName { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public Address Address { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public AppUser CreatedBy { get; set; }

        public ICollection<Account> Accounts { get; set; }

        public ICollection<CreditCard> CreditCards { get; set; }
    }
}
