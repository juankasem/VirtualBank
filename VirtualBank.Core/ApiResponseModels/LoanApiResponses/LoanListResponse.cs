using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.LoanApiResponses
{
    public class LoanListResponse
    {
        public ImmutableList<LoanResponse> Loans { get; }

        public int TotalCount { get; }


        public LoanListResponse(ImmutableList<LoanResponse> loans, int totalCount)
        {
            Loans = loans.IsEmpty ? ImmutableList<LoanResponse>.Empty : loans;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
