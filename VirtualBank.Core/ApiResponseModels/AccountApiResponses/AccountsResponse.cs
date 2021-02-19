using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

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
