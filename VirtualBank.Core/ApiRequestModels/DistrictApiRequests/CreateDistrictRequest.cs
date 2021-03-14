using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.DistrictApiRequests
{
    public class CreateDistrictRequest
    {
        [Required]
        public int CityId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }


        public CreateDistrictRequest(int cityId, string name)
        {
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
        }
    }
}
