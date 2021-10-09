using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Domain.Models
{
    public class BankAccountCurrency
    {
        public int Id { get; }

        public string Code { get; }

        public string Symbol { get; }


        public BankAccountCurrency(int id, string code, string symbol)
        {
            Id = id;
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Symbol = Throw.ArgumentNullException.IfNull(symbol, nameof(symbol));
        }
    }
}