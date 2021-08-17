using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses
{
    public class CashTransactionListResponse
    {
        public ImmutableList<CashTransactionResponse> CashTransactions { get; }

        public int TotalCount { get; }


        public CashTransactionListResponse(ImmutableList<CashTransactionResponse> cashTransactions, int totalCount)
        {
            CashTransactions = cashTransactions.IsEmpty ? ImmutableList<CashTransactionResponse>.Empty : cashTransactions;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
