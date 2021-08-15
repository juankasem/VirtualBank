using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models
{
    public class CreditedFunds
    {
        public Amount Amount { get; set; }

        public string Currency { get; set; }

        public CreditedFunds(Amount amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
}
