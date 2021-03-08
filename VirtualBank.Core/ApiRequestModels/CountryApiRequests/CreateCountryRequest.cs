using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.CountryApiRequests
{
    public class CreateCountryRequest
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(8)]
        public string Code { get; set; }

        public CreateCountryRequest(string name, string code)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
        }
    }
}
