using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Domain.Models;

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
             CreateCreationInfo(branch.CreatedBy, branch.CreatedOn),
             CreateModificationInfo(branch.LastModifiedBy, branch.LastModifiedOn));


        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string lastModifiedBy, DateTime lastModifiedOn) => new(lastModifiedBy, lastModifiedOn);


    }
}
