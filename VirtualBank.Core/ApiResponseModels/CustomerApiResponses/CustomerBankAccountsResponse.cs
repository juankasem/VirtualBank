using System;
using System.Collections.Immutable;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class CustomerBankAccountsResponse
    {
        public int Id { get; }

        public string FirstName { get; }

        public string MiddleName { get;  }

        public string LastName { get;  }

        public ImmutableList<BankAccountResponse> BankAccounts { get; }


        public CustomerBankAccountsResponse(int id, str)
        {

        }
    }
}
