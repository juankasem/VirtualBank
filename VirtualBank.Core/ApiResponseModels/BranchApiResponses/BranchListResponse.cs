using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.BranchApiResponses
{
    public class BranchListResponse
    {
        public ImmutableList<BranchResponse> Branches { get; }

        public int TotalCount { get; }


        public BranchListResponse(ImmutableList<BranchResponse> branches, int totalCount)
        {
            Branches = branches.IsEmpty ? ImmutableList<BranchResponse>.Empty : branches;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
