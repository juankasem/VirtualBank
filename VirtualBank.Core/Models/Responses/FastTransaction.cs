using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.Models.Responses
{
    public class FastTransaction
    {
        public int Id { get; }

        public string IBAN { get; }

        public RecipientDetails RecipientDetails { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public FastTransaction(int id, string iban, RecipientDetails recipientDetails,
                               CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            RecipientDetails = Throw.ArgumentNullException.IfNull(recipientDetails, nameof(recipientDetails));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}