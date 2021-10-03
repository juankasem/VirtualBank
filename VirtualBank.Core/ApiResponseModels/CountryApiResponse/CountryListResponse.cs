using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.CountryApiResponse
{
    public class CountryListResponse
    {
        public ImmutableList<Country> Countries { get; }

        public int TotalCount { get; }


        public CountryListResponse(ImmutableList<Country> countries, int totalCount)
        {
            Countries = countries.IsEmpty ? ImmutableList<Country>.Empty : countries;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
