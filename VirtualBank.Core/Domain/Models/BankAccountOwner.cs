using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Domain.Models
{
    public class BankAccountOwner
    {
        public int CustomerId { get; }

        public string FullName { get; }

        public Gender Gender { get; set; }


        public BankAccountOwner(int customerId, string fullName, Gender gender)
        {
            CustomerId = Throw.ArgumentNullException.IfNull(customerId, nameof(customerId));
            FullName = Throw.ArgumentNullException.IfNull(fullName, nameof(fullName));
            Gender = Throw.ArgumentNullException.IfNull(gender, nameof(gender));
        }
    }
}