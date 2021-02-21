using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiRequestModels.CustomerApiRequests
{
    public class CreateCustomerRequest
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
    }
}
