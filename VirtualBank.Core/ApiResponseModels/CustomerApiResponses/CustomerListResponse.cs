using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class CustomerListResponse
    {
        public ImmutableList<CustomerResponse> Customers { get; }

        public int TotalCount { get; }


        public CustomerListResponse(ImmutableList<CustomerResponse> customers, int totalCount)
        {
            Customers = customers.IsEmpty ? ImmutableList<CustomerResponse>.Empty : customers;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
