using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CreditCardApiResponses
{
    public class CreditCardListResponse
    {
        public ImmutableList<CreditCardResponse> CreditCards { get; }

        public int TotalCount { get; }


        public CreditCardListResponse(ImmutableList<CreditCardResponse> creditCards, int totalCount)
        {
            CreditCards = creditCards.IsEmpty ? ImmutableList<CreditCardResponse>.Empty : creditCards;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
