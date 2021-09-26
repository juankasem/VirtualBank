using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.BranchApiResponses
{
    public class BranchResponse
    {
        public Branch Branch { get; }

        public BranchResponse(Branch branch)
        {
            Branch = Throw.ArgumentNullException.IfNull(branch, nameof(branch));
        }
    }
}
