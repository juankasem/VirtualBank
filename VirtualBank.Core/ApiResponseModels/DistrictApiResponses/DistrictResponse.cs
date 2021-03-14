using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.DistrictApiResponses
{
    public class DistrictResponse
    {
        public int Id { get; set; }

        public int CityId { get; set; }

        public string Name { get; }

        public DistrictResponse(int id, int cityId, string name)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
        }

    }
}
