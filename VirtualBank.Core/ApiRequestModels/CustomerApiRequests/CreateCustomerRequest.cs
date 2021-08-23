using System;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Entities;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.CustomerApiRequests
{
    public class CreateCustomerRequest
    {
        public string IdentificationNo { get; set; }

        public IdentificationType IdentificationType { get; set; }

        public string TaxNumber { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FatherName { get; set; }

        public string Nationality { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string UserId { get; set; }

        public Address Address { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public CreateCustomerRequest(string identificationNo, IdentificationType identificationType, string taxNumber,
                                     string firstName, string middleName, string lastName, string fatherName, string nationality,
                                     Gender gender, DateTime birthDate, string userId, Address address, CreationInfo creationInfo)
        {
            IdentificationNo = Throw.ArgumentNullException.IfNull(identificationNo, nameof(identificationNo));
            IdentificationType = Throw.ArgumentNullException.IfNull(identificationType, nameof(identificationType));
            TaxNumber = Throw.ArgumentNullException.IfNull(taxNumber, nameof(taxNumber));
            FirstName = Throw.ArgumentNullException.IfNull(firstName, nameof(firstName));
            MiddleName = Throw.ArgumentNullException.IfNull(middleName, nameof(middleName));
            LastName = Throw.ArgumentNullException.IfNull(lastName, nameof(lastName));
            FatherName = Throw.ArgumentNullException.IfNull(fatherName, nameof(fatherName));
            Nationality = Throw.ArgumentNullException.IfNull(nationality, nameof(nationality));
            Gender = Throw.ArgumentNullException.IfNull(gender, nameof(gender));
            BirthDate = Throw.ArgumentNullException.IfNull(birthDate, nameof(birthDate));
            UserId = Throw.ArgumentNullException.IfNull(userId, nameof(userId));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
