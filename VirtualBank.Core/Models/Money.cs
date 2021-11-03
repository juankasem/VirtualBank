using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.Models
{
    public class Money
    {
        public Amount Amount { get; set; }

        public MoneyCurrency Currency { get; set; }

        public Money(Amount amount, MoneyCurrency currency)
        {
            Amount = amount;
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
        }
    }
}