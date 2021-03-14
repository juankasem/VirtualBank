using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CityApiResponses
{
    public class CityListResponse
    {
        public ImmutableList<CityResponse> Cities { get; }

        public int TotalCount { get; }


        public CityListResponse(ImmutableList<CityResponse> cities, int totalCount)
        {
            Cities = cities.IsEmpty ? ImmutableList<CityResponse>.Empty : cities;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
