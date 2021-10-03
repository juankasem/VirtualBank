using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.CityApiResponses
{
    public class CityListResponse
    {
        public ImmutableList<City> Cities { get; }

        public int TotalCount { get; }


        public CityListResponse(ImmutableList<City> cities, int totalCount)
        {
            Cities = cities.IsEmpty ? ImmutableList<City>.Empty : cities;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
