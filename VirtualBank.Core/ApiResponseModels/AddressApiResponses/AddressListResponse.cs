using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.AddressApiResponses
{
    public class AddressListResponse
    {
        public ImmutableList<AddressResponse> Addresses { get; }

        public int TotalCount { get; }


        public AddressListResponse(ImmutableList<AddressResponse> addresses, int totalCount)
        {
            Addresses = addresses.IsEmpty ? ImmutableList<AddressResponse>.Empty : addresses;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
