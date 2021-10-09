using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Domain.Models
{
    public class BankAccountBranch
    {
        public int Id { get; }

        public string Code { get; }

        public string Name { get; }

        public string City { get; }


        public BankAccountBranch(int id, string code, string name, string city)
        {
            Id = id;
            Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
            Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
            City = Throw.ArgumentNullException.IfNull(city, nameof(city));
        }
    }
}