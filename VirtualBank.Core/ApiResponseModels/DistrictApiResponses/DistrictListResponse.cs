using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.DistrictApiResponses
{
    public class DistrictListResponse
    {
        public ImmutableList<District> Districts { get; }

        public int TotalCount { get; }

        public DistrictListResponse(ImmutableList<District> districts, int totalCount)
        {
            Districts = districts.IsEmpty ? ImmutableList<District>.Empty : districts;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
