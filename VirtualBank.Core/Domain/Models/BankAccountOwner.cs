using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Domain.Models
{
    public class BankAccountOwner
    {
        public int CustomerId { get; }

        public string FullName { get; }


        public BankAccountOwner(int customerId, string fullName)
        {
            CustomerId = Throw.ArgumentNullException.IfNull(customerId, nameof(customerId));
            FullName = Throw.ArgumentNullException.IfNull(fullName, nameof(fullName));
        }
    }
}