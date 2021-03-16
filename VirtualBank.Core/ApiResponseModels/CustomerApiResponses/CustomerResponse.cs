using System;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class CustomerResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Nationality { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string UserId { get; set; }

        public AddressResponse Address { get; set; }


        public CustomerResponse(int id, string fullName, string nationality,Gender gender,
                                DateTime birthDate, string userId, AddressResponse address)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            FullName = Throw.ArgumentNullException.IfNull(fullName, nameof(fullName));
            Nationality = Throw.ArgumentNullException.IfNull(nationality, nameof(nationality));
            Gender = Throw.ArgumentNullException.IfNull(gender, nameof(gender));
            BirthDate = Throw.ArgumentNullException.IfNull(birthDate, nameof(birthDate));
            UserId = Throw.ArgumentNullException.IfNull(userId, nameof(userId));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
        }
    }
}
