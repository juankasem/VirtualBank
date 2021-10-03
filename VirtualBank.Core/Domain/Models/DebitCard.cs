using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class DebitCard
    {
        public int Id { get; }

        public string DebitCardNo { get; }

        public DateTime ExpirationDate { get; }

        public string IBAN { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public DebitCard(int id, string debitCardNo, DateTime expirationDate, string iban,
                         CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            DebitCardNo = Throw.ArgumentNullException.IfNull(debitCardNo, nameof(debitCardNo));
            ExpirationDate = Throw.ArgumentNullException.IfNull(expirationDate, nameof(expirationDate));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            CreationInfo = Throw.ArgumentException.IfDefault(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentException.IfDefault(modificationInfo, nameof(modificationInfo));
        }
    }
}