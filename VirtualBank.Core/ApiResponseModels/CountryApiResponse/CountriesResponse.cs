using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CountryApiResponse
{
    public class CountriesResponse
    {
        public ImmutableList<CountryResponse> Countries { get; }

        public int TotalCount { get; }


        public CountriesResponse(ImmutableList<CountryResponse> countries, int totalCount)
        {
            Countries = countries.IsEmpty ? ImmutableList<CountryResponse>.Empty : countries;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
