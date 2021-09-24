using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.BankAccountApiResponses
{
    public class BankAccountListResponse
    {
        public ImmutableList<BankAccount> BankAccounts { get; }

        public int TotalCount { get; }

        public BankAccountListResponse(ImmutableList<BankAccount> bankAccounts, int totalCount)
        {
            BankAccounts = bankAccounts.IsEmpty ? ImmutableList<BankAccount>.Empty : bankAccounts;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
