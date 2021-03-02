using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.BranchApiResponses
{
    public class BranchResponse
    {
        public string Name { get; }

        public string Code { get; }

        public Address Address { get;  }

        public BranchResponse(string name, string code, Address address)
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
        }
    }
}
