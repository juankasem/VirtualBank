using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models
{
    public class AppUser : IdentityUser
    {

        [Required]
        [MinLength(8)]
        public string IdentificationNo { get; set; }

        [Required]
        public IdentificationType Type { get; set; }

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

    }
}
