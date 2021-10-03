using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests
{
    public class CreateCashTransactionRequest
    {
        public CashTransactionType Type { get; set; }

        public BankAssetType InitiatedBy { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string RecipientFirstName { get; set; }

        public string RecipientLastName { get; set; }

        public Money DebitedFunds { get; set; }

        public Money Fees { get; set; }

        public string Description { get; set; }

        public PaymentType PaymentType { get; set; }

        public DateTime TransactionDate { get; set; }

#nullable enable
        public string? CreditCardNo { get; set; }

#nullable enable
        public string? DebitCardNo { get; set; }

#nullable enable
        public string? PIN { get; set; }

        public CreationInfo CreationInfo { get; set; }


        public CreateCashTransactionRequest(CashTransactionType type, BankAssetType initiatedBy, string from, string to,
                                            string recipientFirstName, string recipientLastName, Money debitedFunds,
                                            Money fees, string description, PaymentType paymentType, DateTime transactionDate,
                                            string creditCardNo, string debitCardNo, string pin, CreationInfo creationInfo)
        {
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            InitiatedBy = Throw.ArgumentNullException.IfNull(initiatedBy, nameof(initiatedBy));
            From = from;
            To = to;
            RecipientFirstName = recipientFirstName;
            RecipientLastName = recipientLastName;
            DebitedFunds = Throw.ArgumentNullException.IfNull(debitedFunds, nameof(debitedFunds));
            Fees = Throw.ArgumentNullException.IfNull(fees, nameof(fees));
            Description = description;
            PaymentType = Throw.ArgumentNullException.IfNull(paymentType, nameof(paymentType));
            TransactionDate = Throw.ArgumentNullException.IfNull(transactionDate, nameof(transactionDate));
            CreditCardNo = creditCardNo;
            DebitCardNo = debitCardNo;
            PIN = pin;
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
