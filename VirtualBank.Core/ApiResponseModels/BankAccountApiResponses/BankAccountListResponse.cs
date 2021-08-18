using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.BankAccountApiResponses
{
    public class BankAccountListResponse
    {
        public ImmutableList<BankAccountResponse> BankAccounts { get; }

        public int TotalCount { get; }


        public BankAccountListResponse(ImmutableList<BankAccountResponse> bankAccounts, int totalCount)
        {
            BankAccounts = bankAccounts.IsEmpty ? ImmutableList<BankAccountResponse>.Empty : bankAccounts;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
