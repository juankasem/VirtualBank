using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;
using Address = VirtualBank.Core.Entities.Address;

namespace VirtualBank.Core.ApiRequestModels.BranchApiRequests
{
    public class CreateBranchRequest
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Phone { get; set; }

        public Address Address { get; set; }

        public CreationInfo CreationInfo { get; set; }


        public CreateBranchRequest(string name, string code, string phone, Address address, CreationInfo creationInfo)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Phone = Throw.ArgumentNullException.IfNull(phone, nameof(phone));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
