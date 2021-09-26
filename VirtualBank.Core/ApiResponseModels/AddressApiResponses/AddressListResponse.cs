using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.AddressApiResponses
{
    public class AddressListResponse
    {
        public ImmutableList<Address> Addresses { get; }

        public int TotalCount { get; }


        public AddressListResponse(ImmutableList<Address> addresses, int totalCount)
        {
            Addresses = addresses.IsEmpty ? ImmutableList<Address>.Empty : addresses;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
