using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses
{
    public class CashTransactionResponse
    {
        /// <summary>
        /// the bank asset that initiated the transaction
        /// </summary>
        public BankAssetType InitiatedBy { get; set; }

        /// <summary>
        /// the bank account that sent the money
        /// </summary>
        public string From { get; }

        /// <summary>
        /// the bank account that received the money
        /// </summary>
        public string To { get; }

        /// <summary>
        /// sender full name
        /// </summary>
        public string Sender { get; }

        /// <summary>
        /// recipient full name
        /// </summary>
        public string Recipient { get; }

        /// <summary>
        /// Amount of money
        /// </summary>
        public Money DebitedFunds { get; set; }

        /// <summary>
        /// Amount of transfer fees 
        /// </summary>
        public Money Fees { get; set; }

        /// <summary>
        /// payment for
        /// </summary>
        public PaymentType PaymentType { get; }

        /// <summary>
        /// description of transaction
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// remaining balance after transaction
        /// </summary>
        public decimal RemainingBalance { get; }

        /// <summary>
        /// date & time of the transaction
        /// </summary>
        public DateTime CreatedOn { get; }

        /// <summary>
        /// user who created the transaction
        /// </summary>
        public string CreatedBy { get; set; }

        public CashTransactionResponse(string from, string to, string sender, string recipient, Money debitedFunds, Money fees,
                                       PaymentType paymentType, string description,BankAssetType initiatedBy, decimal remainingBalance,
                                       DateTime createdOn, string createdBy)
        {
            From = from;
            To = to;
            DebitedFunds = Throw.ArgumentNullException.IfNull(debitedFunds, nameof(debitedFunds));
            Fees = Throw.ArgumentNullException.IfNull(fees, nameof(fees));
            Sender = Throw.ArgumentNullException.IfNull(sender, nameof(sender));
            Recipient = Throw.ArgumentNullException.IfNull(recipient, nameof(recipient));
            PaymentType = Throw.ArgumentNullException.IfNull(paymentType, nameof(paymentType));
            Description = Throw.ArgumentNullException.IfNull(description, nameof(description));
            InitiatedBy = Throw.ArgumentNullException.IfNull(initiatedBy, nameof(initiatedBy));
            RemainingBalance = Throw.ArgumentNullException.IfNull(remainingBalance, nameof(remainingBalance));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            CreatedBy = Throw.ArgumentNullException.IfNull(createdBy, nameof(createdBy));
        }
    }
}
