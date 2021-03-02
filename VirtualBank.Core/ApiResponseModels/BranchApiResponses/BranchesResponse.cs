using System;
using System.Collections.Immutable;

namespace VirtualBank.Core.ApiResponseModels.BranchApiResponses
{
    public class BranchesResponse
    {
        public ImmutableArray<BranchResponse> Branches { get; }

        public BranchesResponse(ImmutableArray<BranchResponse> branches)
        {
            Branches = branches.IsDefault ? ImmutableArray<BranchResponse>.Empty : branches;
        }
    }
}
