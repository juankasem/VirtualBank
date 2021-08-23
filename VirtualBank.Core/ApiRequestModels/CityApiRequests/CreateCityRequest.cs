using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.CityApiRequests
{
    public class CreateCityRequest
    {
        public int CountryId { get; set; }

        public string Name { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public CreateCityRequest(int countryId, string name, CreationInfo creationInfo)
        {
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
