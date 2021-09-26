using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.BranchApiResponses
{
    public class BranchListResponse
    {
        public ImmutableList<Branch> Branches { get; }

        public int TotalCount { get; }


        public BranchListResponse(ImmutableList<Branch> branches, int totalCount)
        {
            Branches = branches.IsEmpty ? ImmutableList<Branch>.Empty : branches;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
