using System;
using System.Collections.Immutable;

namespace VirtualBank.Core.ApiResponseModels.CountryApiResponse
{
    public class CountriesResponse
    {
        public ImmutableArray<CountryResponse> Countries { get; }

        public CountriesResponse(ImmutableArray<CountryResponse> countries)
        {
            Countries = countries.IsDefault ? ImmutableArray<CountryResponse>.Empty : countries;
        }
    }
}
