using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CityApiResponses
{
    public class CityResponse
    {
        public int Id { get; set; }

        public int CountryId { get; set; }

        public string Name { get; }

        public string Code { get; }

        public CityResponse(int id, int countryId, string name, string code)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
        }
    }
}
