using System;
using System.Collections.Immutable;
using VirtualBank.Core.ApiResponseModels.BankAccountApiResponses;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class CustomerWithBankAccountsResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Nationality { get; set; }

        public Gender Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public Address Address { get; set; }

        public ImmutableList<BankAccountResponse> BankAccounts { get; }


        public CustomerWithBankAccountsResponse(int id, string fullName, string nationality,
                                                Gender gender, DateTime birthDate, Address address)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            FullName = Throw.ArgumentNullException.IfNull(fullName, nameof(fullName));
            Nationality = Throw.ArgumentNullException.IfNull(nationality, nameof(nationality));
            Gender = Throw.ArgumentNullException.IfNull(gender, nameof(gender));
            BirthDate = Throw.ArgumentNullException.IfNull(birthDate, nameof(birthDate));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
        }
    }
}
