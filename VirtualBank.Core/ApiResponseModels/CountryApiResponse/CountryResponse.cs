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

        public ImmutableList<CityResponse> Cities{ get;}

        public CountryResponse(int id, string name, string code, ImmutableList<CityResponse> cities)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Cities = cities;
        }
    }
}
