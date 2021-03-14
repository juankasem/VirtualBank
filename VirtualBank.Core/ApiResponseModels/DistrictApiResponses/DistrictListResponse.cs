using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.DistrictApiResponses
{
    public class DistrictListResponse
    {
        public ImmutableList<DistrictResponse> Districts { get; }

        public int TotalCount { get; }


        public DistrictListResponse(ImmutableList<DistrictResponse> districts, int totalCount)
        {
            Districts = districts.IsEmpty ? ImmutableList<DistrictResponse>.Empty : districts;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
