using System;
using System.Collections;
using System.Collections.Immutable;
using VirtualBank.Core.ApiResponseModels.CityApiResponses;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CountryApiResponse
{
    public class CountryResponse
    {
        public int Id { get; }

        public string Name { get; }

        public string Code { get; }

        public ImmutableArray<CityResponse> Cities{ get;}

        public CountryResponse(int id, string name, string code, ImmutableArray<CityResponse> cities)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Cities = cities;
        }
    }
}
