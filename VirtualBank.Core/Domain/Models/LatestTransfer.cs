using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class LatestTransfer
    {
        /// <summary>
        /// the bank account that received the money
        /// </summary>
        public string To { get; }

        /// <summary>
        /// recipient full name
        /// </summary>
        public string Recipient { get; }

        /// <summary>
        /// Amount of money
        /// </summary>
        public Amount Amount { get; set; }

        /// <summary>
        /// date and time of the transaction
        /// </summary>
        public DateTime TransactionDate { get; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }


        public LatestTransfer(string to, string recipient, Amount amount, DateTime transactionDate,
                             CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            To = Throw.ArgumentNullException.IfNull(to, nameof(to));
            Recipient = Throw.ArgumentNullException.IfNull(recipient, nameof(recipient));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            TransactionDate = Throw.ArgumentNullException.IfNull(transactionDate, nameof(transactionDate));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}