using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models
{
    public class DebitedFunds
    {
        public Amount Amount { get; set; }

        public string Currency { get; set; }

        public DebitedFunds(Amount amount, string currency)
        {
            Amount = amount;
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
        }
    }
}
