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

        [Required]
        [MaxLength(8)]
        public string Code { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public CreateCityRequest(int countryId, string name, string code, string phone)
        {
            CountryId = Throw.ArgumentNullException.IfNull(countryId, nameof(countryId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Phone = Throw.ArgumentNullException.IfNull(phone, nameof(phone));
        }
    }
}
