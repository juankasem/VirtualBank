using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.DebitCardApiResponses
{
    public class DebitCardListResponse
    {
        public ImmutableList<DebitCardResponse> DebitCards { get; }

        public int TotalCount { get; }


        public DebitCardListResponse(ImmutableList<DebitCardResponse> debitCards, int totalCount)
        {
            DebitCards = debitCards.IsEmpty ? ImmutableList<DebitCardResponse>.Empty : debitCards;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
