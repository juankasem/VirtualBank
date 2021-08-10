using System;
using VirtualBank.Core.ApiResponseModels.AddressApiResponses;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.BranchApiResponses
{
    public class BranchResponse
    {
        public int Id { get;  }

        public string Name { get; }

        public string Code { get; }

        public string Phone { get; }

        public AddressResponse Address { get;  }

        public BranchResponse(int id, string name, string code, string phone, AddressResponse address)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Phone = Throw.ArgumentNullException.IfNull(phone, nameof(phone));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
        }
    }
}
