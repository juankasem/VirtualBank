using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.CountryApiResponse
{
    public class CountryResponse
    {
        public Country Country { get; }

        public CountryResponse(Country country)
        {
            Country = Throw.ArgumentNullException.IfNull(country, nameof(country));
        }
    }
}
