using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class CustomerListResponse
    {
        public ImmutableList<Customer> Customers { get; }

        public int TotalCount { get; }


        public CustomerListResponse(ImmutableList<Customer> customers, int totalCount)
        {
            Customers = customers.IsEmpty ? ImmutableList<Customer>.Empty : customers;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
