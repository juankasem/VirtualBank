using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Entities;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.CustomerApiRequests
{
    public class CreateCustomerRequest
    {
        [Required]
        [MinLength(50)]
        public string IdentificationNo { get; set; }

        [Required]
        public IdentificationType IdentificationType { get; set; }

        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(150)]
        public string FatherName { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nationality { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public Address Address { get; set; }


        public CreateCustomerRequest(string identificationNo, IdentificationType identificationType,
                                     string firstName, string middleName, string lastName, string fatherName,
                                     string nationality, Gender gender, DateTime birthDate, string userId, Address address)
        {
            IdentificationNo = Throw.ArgumentNullException.IfNull(identificationNo, nameof(identificationNo));
            IdentificationType = Throw.ArgumentNullException.IfNull(identificationType, nameof(identificationType));
            FirstName = Throw.ArgumentNullException.IfNull(firstName, nameof(firstName));
            MiddleName = Throw.ArgumentNullException.IfNull(middleName, nameof(middleName));
            LastName = Throw.ArgumentNullException.IfNull(lastName, nameof(lastName));
            FatherName = Throw.ArgumentNullException.IfNull(fatherName, nameof(fatherName));
            Nationality = Throw.ArgumentNullException.IfNull(nationality, nameof(nationality));
            Gender= Throw.ArgumentNullException.IfNull(gender, nameof(gender));
            BirthDate = Throw.ArgumentNullException.IfNull(birthDate, nameof(birthDate));
            UserId = Throw.ArgumentNullException.IfNull(userId, nameof(userId));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
        }
    }
}
