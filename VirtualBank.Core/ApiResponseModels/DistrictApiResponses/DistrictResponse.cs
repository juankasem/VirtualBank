using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.DistrictApiResponses
{
    public class DistrictResponse
    {
        public District District { get; }

        public DistrictResponse(District district)
        {
            District = Throw.ArgumentNullException.IfNull(district, nameof(district));
        }
    }
}
