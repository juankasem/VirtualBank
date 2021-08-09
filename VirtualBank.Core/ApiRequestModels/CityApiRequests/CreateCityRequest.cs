using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.CityApiRequests
{
    public class CreateCityRequest
    {
        public int CountryId { get; set; }

        public string Name { get; set; }


        public CreateCityRequest(int countryId, string name)
        {
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
        }
    }
}
