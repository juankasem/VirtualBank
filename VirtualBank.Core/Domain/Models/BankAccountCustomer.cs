using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Domain.Models
{
    public class BankAccountCustomer
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string IBAN { get; set; }

        public BankAccountCustomer(int customerId, string customerName, string iban)
        {
            CustomerId = customerId;
            CustomerName = Throw.ArgumentNullException.IfNull(customerName, nameof(customerName));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
        }
    }
}