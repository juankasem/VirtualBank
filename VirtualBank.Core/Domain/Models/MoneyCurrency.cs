using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Domain.Models
{
    public class MoneyCurrency
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Symbol { get; set; }

        public MoneyCurrency(int id, string code, string symbol)
        {
            Id = id;
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Symbol = Throw.ArgumentNullException.IfNull(symbol, nameof(symbol));
        }
    }
}