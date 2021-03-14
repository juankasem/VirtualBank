using System;
using System.Collections.Immutable;

namespace VirtualBank.Core.ApiResponseModels.CountryApiResponse
{
    public class CountriesResponse
    {
        public ImmutableList<CountryResponse> Countries { get; }

        public CountriesResponse(ImmutableList<CountryResponse> countries)
        {
            Countries = countries.IsEmpty ? ImmutableList<CountryResponse>.Empty : countries;
        }
    }
}
