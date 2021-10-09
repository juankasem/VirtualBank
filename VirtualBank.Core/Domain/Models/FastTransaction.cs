using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class FastTransaction
    {
        public int Id { get; }

        public string IBAN { get; }

        public RecipientDetails RecipientDetails { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }


        public FastTransaction(int id, string iban, RecipientDetails recipientDetails,
                               CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            RecipientDetails = Throw.ArgumentNullException.IfNull(recipientDetails, nameof(recipientDetails));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }

        public Core.Entities.FastTransaction ToEntity() =>
          new Core.Entities.FastTransaction(Id,
                                            IBAN,
                                            RecipientDetails.BankAccountId,
                                            RecipientDetails.RecipientFullName,
                                            RecipientDetails.RecipientShortName,
                                            RecipientDetails.AmountToTransfer.Amount,
                                            CreationInfo.CreatedBy,
                                            CreationInfo.CreatedOn,
                                            ModificationInfo.ModifiedBy,
                                            ModificationInfo.LastModifiedOn);
    }
}