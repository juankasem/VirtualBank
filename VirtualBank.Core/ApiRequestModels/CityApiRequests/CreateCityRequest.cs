using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.CityApiRequests
{
    public class CreateCityRequest
    {
        [Required]
        public int CountryId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }


        public CreateCityRequest(int countryId, string name)
        {
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
        }
    }
}
