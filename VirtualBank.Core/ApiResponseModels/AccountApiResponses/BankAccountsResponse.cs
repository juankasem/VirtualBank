using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.AccountApiResponses
{
    public class BankAccountsResponse
    {
        public ImmutableArray<BankAccountResponse> BankAccounts { get; }

        public BankAccountsResponse(ImmutableArray<BankAccountResponse> bankAccounts)
        {
            BankAccounts = bankAccounts.IsDefault ? ImmutableArray<BankAccountResponse>.Empty : bankAccounts;
        }
    }
}
