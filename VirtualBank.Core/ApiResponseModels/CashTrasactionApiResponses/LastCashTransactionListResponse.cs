using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses
{
    public class LastCashTransactionListResponse
    {
        public ImmutableList<LastCashTransactionResponse> LastCashTransactions { get; }

        public int TotalCount { get; }


        public LastCashTransactionListResponse(ImmutableList<LastCashTransactionResponse> lastCashTransactions, int totalCount)
        {
            LastCashTransactions = lastCashTransactions.IsEmpty ? ImmutableList<LastCashTransactionResponse>.Empty : lastCashTransactions;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
