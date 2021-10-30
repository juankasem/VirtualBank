using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.LoanApiResponses
{
    public class LoanListResponse
    {
        public ImmutableList<Loan> Loans { get; }

        public int TotalCount { get; }


        public LoanListResponse(ImmutableList<Loan> loans, int totalCount)
        {
            Loans = loans.IsEmpty ? ImmutableList<Loan>.Empty : loans;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
