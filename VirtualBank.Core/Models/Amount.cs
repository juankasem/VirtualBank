using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models
{
    public class Amount
    {
        public decimal Value { get; }

        public Amount(decimal value) =>
            Value = Throw.ArgumentOutOfRangeException.IfLessThan(value, 0, nameof(value));


        public Amount Add(Amount amount) => new Amount(Value + amount);

        public Amount Subtract(Amount amount) => new Amount(Value - amount);

        public static implicit operator decimal(Amount? amount) => amount?.Value ?? 0;
    }
}
