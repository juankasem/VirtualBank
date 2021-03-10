using System;
using System.Collections.Immutable;

namespace VirtualBank.Core.ApiResponseModels.CityApiResponses
{
    public class CitiesResponse
    {
        public ImmutableArray<CityResponse> Cities { get; }

        public CitiesResponse(ImmutableArray<CityResponse> cities)
        {
            Cities = cities.IsDefault ? ImmutableArray<CityResponse>.Empty : cities;
        }
    }
}
