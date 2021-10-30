using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Api.Helpers.Methods;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IBranchMapper
    {
        Branch MapToResponseModel(VirtualBank.Core.Entities.Branch branch);
    }

    public class BranchMapper : IBranchMapper
    {
        private readonly IAddressMapper _addressMapper;

        public BranchMapper(IAddressMapper addressMapper)
        {
            _addressMapper = addressMapper;
        }
        public Branch MapToResponseModel(Core.Entities.Branch branch) =>
            new(
             branch.Id,
             branch.Code,
             branch.Name,
             branch.Phone,
             _addressMapper.MapToResponseModel(branch.Address),
             Utils.CreateCreationInfo(branch.CreatedBy, branch.CreatedOn),
             Utils.CreateModificationInfo(branch.LastModifiedBy, branch.LastModifiedOn));
    }
}
