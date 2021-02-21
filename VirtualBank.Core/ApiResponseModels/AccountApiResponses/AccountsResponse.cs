using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.AccountApiResponses
{
    public class AccountsResponse
    {
        public ImmutableArray<Account> Accounts { get; }

        public AccountsResponse(ImmutableArray<Account> accounts)
        {
            Accounts = accounts.IsDefault ? ImmutableArray<Account>.Empty : accounts;
        }
    }
}
