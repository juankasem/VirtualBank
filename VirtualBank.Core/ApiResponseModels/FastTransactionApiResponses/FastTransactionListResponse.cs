using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses
{
    public class FastTransactionListResponse
    {
        public ImmutableList<FastTransaction> FastTransactions { get; }

        public int TotalCount { get; }

        public FastTransactionListResponse(ImmutableList<FastTransaction> fastTransactions, int totalCount)
        {
            FastTransactions = fastTransactions.IsEmpty ? ImmutableList<FastTransaction>.Empty : fastTransactions;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
