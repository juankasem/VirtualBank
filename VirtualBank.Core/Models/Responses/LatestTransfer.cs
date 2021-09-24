using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Models.Responses
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


        public LatestTransfer(string to, string recipient, Amount amount, DateTime transactionDate)
        {
            To = Throw.ArgumentNullException.IfNull(to, nameof(to));
            Recipient = Throw.ArgumentNullException.IfNull(recipient, nameof(recipient));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            TransactionDate = Throw.ArgumentNullException.IfNull(transactionDate, nameof(transactionDate));
        }
    }
}