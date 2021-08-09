using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.DistrictApiRequests
{
    public class CreateDistrictRequest
    {
        public int CityId { get; set; }

        public string Name { get; set; }


        public CreateDistrictRequest(int cityId, string name)
        {
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
        }
    }
}
