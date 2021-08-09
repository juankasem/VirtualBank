using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiRequestModels.BranchApiRequests
{
    public class CreateBranchRequest
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Phone { get; set; }

        public Address Address { get; set; }

        public CreateBranchRequest(string name, string code, string phone, Address address) 
        {
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Phone = Throw.ArgumentNullException.IfNull(phone, nameof(phone));
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
        }
    }
}
