using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class CustomerResponse
    {
        public Customer Customer { get; }


        public CustomerResponse(Customer customer)
        {
            Customer = Throw.ArgumentNullException.IfNull(customer, nameof(customer));
        }
    }
}
