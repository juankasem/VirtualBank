using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.BranchApiResponses
{
    public class BranchResponse
    {
        public string Name { get; }

        public string Code { get; }

        public string Phone { get; }

        public Address Address { get;  }

        public BranchResponse(string name, string code, string phone, Address address)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Phone = Throw.ArgumentNullException.IfNull(phone, nameof(phone));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
        }
    }
}
