using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.DistrictApiRequests
{
    public class CreateDistrictRequest
    {
        public int CityId { get; set; }

        public string Name { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }

        public CreateDistrictRequest(int cityId, string name, CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            CityId = Throw.ArgumentNullException.IfNull(cityId, nameof(cityId));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}
