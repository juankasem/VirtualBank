using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IBranchMapper
    {
        Branch MapToResponseModel(VirtualBank.Core.Entities.Branch branch);
    }

    public class BranchMapper : IBranchMapper
    {
        public Branch MapToResponseModel(Core.Entities.Branch branch)
        {
            throw new System.NotImplementedException();
        }
    }
}
