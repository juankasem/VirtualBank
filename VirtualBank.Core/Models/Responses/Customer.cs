using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models.Responses
{
    public class Customer
    {
        public int Id { get; }

        public string FullName { get; set; }

        public string Nationality { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string UserId { get; set; }

        public Address Address { get; set; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }

        public Customer(int id, string fullName, string nationality, Gender gender,
                        DateTime birthDate, string userId, Address address,
                        CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            FullName = Throw.ArgumentNullException.IfNull(fullName, nameof(fullName));
            Nationality = Throw.ArgumentNullException.IfNull(nationality, nameof(nationality));
            Gender = Throw.ArgumentNullException.IfNull(gender, nameof(gender));
            BirthDate = Throw.ArgumentNullException.IfNull(birthDate, nameof(birthDate));
            UserId = Throw.ArgumentNullException.IfNull(userId, nameof(userId));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }
    }
}