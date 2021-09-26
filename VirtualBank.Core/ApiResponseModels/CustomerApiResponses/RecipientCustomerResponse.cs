using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.CustomerApiResponses
{
    public class RecipientCustomerResponse
    {
        public RecipientCustomer RecipientCustomer { get; }

        public RecipientCustomerResponse(RecipientCustomer recipientCustomer)
        {
            RecipientCustomer = Throw.ArgumentNullException.IfNull(recipientCustomer, nameof(recipientCustomer));
        }
    }
}
