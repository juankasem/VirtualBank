using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.CityApiResponses
{
    public class CityResponse
    {
        public City City { get; }

        public CityResponse(City city)
        {
            City = Throw.ArgumentNullException.IfNull(city, nameof(city));
        }
    }
}
