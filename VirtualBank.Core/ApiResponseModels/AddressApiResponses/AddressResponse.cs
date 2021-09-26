using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

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
