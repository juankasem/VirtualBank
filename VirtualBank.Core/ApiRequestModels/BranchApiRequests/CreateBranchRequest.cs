using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiRequestModels.BranchApiRequests
{
    public class CreateBranchRequest
    {
        public Branch Branch { get; }

        public CreateBranchRequest(Branch branch)
        {
            Branch = Throw.ArgumentNullException.IfNull(branch, nameof(branch));
        }
    }
}
