using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Domain.Models
{
    public class RecipientCustomer
    {
        public string FirstName { get; }

        public string LastName { get; }


        public RecipientCustomer(string firstName, string lastName)
        {
            FirstName = Throw.ArgumentNullException.IfNull(firstName, nameof(firstName));
            LastName = Throw.ArgumentNullException.IfNull(lastName, nameof(lastName));
        }
    }
}