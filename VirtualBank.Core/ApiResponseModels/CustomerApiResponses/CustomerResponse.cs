using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class CustomerResponse
    {
        public Customer Customer { get; set; }

        public CustomerResponse(Customer customer)
        {
            Customer = Throw.ArgumentNullException.IfNull(customer, nameof(customer));
        }
    }
}
