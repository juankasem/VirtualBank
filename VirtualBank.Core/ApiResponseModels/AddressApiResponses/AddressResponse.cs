using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.AddressApiResponses
{
    public class AddressResponse
    {
        public Address Address { get; }

        public AddressResponse(Address address)
        {
            Address = Throw.ArgumentNullException.IfNull(address, nameof(address));
        }
    }
}
