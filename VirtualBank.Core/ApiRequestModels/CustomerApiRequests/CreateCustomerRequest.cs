using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Entities;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.CustomerApiRequests
{
    public class CreateCustomerRequest
    {
        public Customer Customer { get; set; }

        public CreateCustomerRequest(Customer customer)
        {
            Customer = Throw.ArgumentNullException.IfNull(customer, nameof(customer));
        }
    }
}
