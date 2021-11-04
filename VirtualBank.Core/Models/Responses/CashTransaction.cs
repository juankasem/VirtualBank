using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models.Responses
{
    public class CashTransaction
    {
        public string Id { get; }

        public string ReferenceNo { get; }

        public CashTransactionType Type { get; }

        public BankAssetType InitiatedBy { get; set; }

        public string From { get; }

        public string To { get; }

        public string Sender { get; }

        public string Recipient { get; }

        public Money DebitedFunds { get; set; }

        public Money Fees { get; set; }

        public PaymentType PaymentType { get; }

        public string Description { get; }

        public Money RemainingBalance { get; }

        public DateTime TransactionDate { get; }

        public CreationInfo CreationInfo { get; }

#nullable enable
        public string? CreditCardNo { get; set; }

#nullable enable
        public string? DebitCardNo { get; set; }


        public CashTransaction(string id, string referenceNo, CashTransactionType Type, BankAssetType initiatedBy,
                               string from, string to, string sender, string recipient, Money debitedFunds, Money fees,
                               PaymentType paymentType, string description, Money remainingBalance, DateTime transactionDate, CreationInfo creationInfo,
                               string? creditCardNo, string? debitCardNo)
        {
            Id = id;
            ReferenceNo = Throw.ArgumentNullException.IfNull(referenceNo, nameof(referenceNo));
            InitiatedBy = Throw.ArgumentNullException.IfNull(initiatedBy, nameof(initiatedBy));
            From = from;
            To = to;
            DebitedFunds = Throw.ArgumentNullException.IfNull(debitedFunds, nameof(debitedFunds));
            Fees = Throw.ArgumentNullException.IfNull(fees, nameof(fees));
            Sender = Throw.ArgumentNullException.IfNull(sender, nameof(sender));
            Recipient = Throw.ArgumentNullException.IfNull(recipient, nameof(recipient));
            PaymentType = Throw.ArgumentNullException.IfNull(paymentType, nameof(paymentType));
            Description = Throw.ArgumentNullException.IfNull(description, nameof(description));
            RemainingBalance = Throw.ArgumentNullException.IfNull(remainingBalance, nameof(remainingBalance));
            TransactionDate = Throw.ArgumentNullException.IfNull(transactionDate, nameof(transactionDate));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            CreditCardNo = creditCardNo;
            DebitCardNo = debitCardNo;
        }
    }
}
