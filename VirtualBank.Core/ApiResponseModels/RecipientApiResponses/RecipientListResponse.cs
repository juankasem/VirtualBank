using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.RecipientApiResponses
{
    public class RecipientListResponse
    {
        public ImmutableList<RecipientResponse> Recipients { get; }

        public int TotalCount { get; }

        public RecipientListResponse(ImmutableList<RecipientResponse> recipients, int totalCount)
        {
            Recipients = recipients.IsEmpty ? ImmutableList<RecipientResponse>.Empty : recipients;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
