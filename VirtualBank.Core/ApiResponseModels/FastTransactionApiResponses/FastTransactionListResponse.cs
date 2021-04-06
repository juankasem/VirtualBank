using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses
{
    public class FastTransactionListResponse
    {
        public ImmutableList<FastTransactionResponse> FastTransactions { get; }

        public int TotalCount { get; }


        public FastTransactionListResponse(ImmutableList<FastTransactionResponse> fastTransactions, int totalCount)
        {
            FastTransactions = fastTransactions.IsEmpty ? ImmutableList<FastTransactionResponse>.Empty : fastTransactions;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
