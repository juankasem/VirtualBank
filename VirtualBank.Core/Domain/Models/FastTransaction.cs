using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class FastTransaction
    {
        public int Id { get; }

        public string IBAN { get; }

        public string BranchName { get; }

        public string RecipientName { get; }

        public string RecipientIBAN { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public FastTransaction(int id, string IBAN, string branchName, string recipientName, string recipientIBAN,
                               CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            IBAN = Throw.ArgumentNullException.IfNull(IBAN, nameof(IBAN));
            BranchName = Throw.ArgumentNullException.IfNull(branchName, nameof(branchName));
            RecipientName = Throw.ArgumentNullException.IfNull(recipientName, nameof(recipientName));
            RecipientIBAN = Throw.ArgumentNullException.IfNull(recipientIBAN, nameof(recipientIBAN));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}