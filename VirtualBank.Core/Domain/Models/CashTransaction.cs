using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class CashTransaction
    {
        public Guid Id { get; }

        public string ReferenceNo { get; }

        public CashTransactionType Type { get; }

        public BankAssetType InitiatedBy { get; set; }

        public string From { get; }

        public string To { get; }

        public Money DebitedFunds { get; set; }

        public Money Fees { get; set; }

        public PaymentType PaymentType { get; }

        public string Description { get; }

        public Money SenderRemainingBalance { get; }

        public Money RecipientRemainingBalance { get; }

        public DateTime TransactionDate { get; }

        public CreationInfo CreationInfo { get; }

#nullable enable
        public string? CreditCardNo { get; set; }

#nullable enable
        public string? DebitCardNo { get; set; }


        public CashTransaction(Guid id, string referenceNo, CashTransactionType type, BankAssetType initiatedBy,
                               string from, string to, Money debitedFunds, Money fees, PaymentType paymentType,
                               string description, Money senderRemainingBalance, Money recipientRemainingBalance,
                               DateTime transactionDate, CreationInfo creationInfo, string? creditCardNo, string? debitCardNo)
        {
            Id = id;
            ReferenceNo = Throw.ArgumentNullException.IfNull(referenceNo, nameof(referenceNo));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            InitiatedBy = Throw.ArgumentNullException.IfNull(initiatedBy, nameof(initiatedBy));
            From = from;
            To = to;
            DebitedFunds = Throw.ArgumentNullException.IfNull(debitedFunds, nameof(debitedFunds));
            Fees = Throw.ArgumentNullException.IfNull(fees, nameof(fees));
            PaymentType = Throw.ArgumentNullException.IfNull(paymentType, nameof(paymentType));
            Description = Throw.ArgumentNullException.IfNull(description, nameof(description));
            SenderRemainingBalance = Throw.ArgumentNullException.IfNull(senderRemainingBalance, nameof(senderRemainingBalance));
            RecipientRemainingBalance = Throw.ArgumentNullException.IfNull(recipientRemainingBalance, nameof(recipientRemainingBalance));
            TransactionDate = Throw.ArgumentNullException.IfNull(transactionDate, nameof(transactionDate));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            CreditCardNo = creditCardNo;
            DebitCardNo = debitCardNo;
        }

        public Core.Entities.CashTransaction ToEntity(decimal senderRemainingBalance, decimal recipientRemainingBalance) =>
            new Core.Entities.CashTransaction(Id,
                                            ReferenceNo,
                                            Type,
                                            InitiatedBy,
                                            From,
                                            To,
                                            DebitedFunds.Amount.Value,
                                            DebitedFunds.Currency,
                                            senderRemainingBalance,
                                            recipientRemainingBalance,
                                            Fees.Amount.Value,
                                            Description,
                                            PaymentType,
                                            TransactionDate,
                                            CreationInfo.CreatedBy,
                                            CreationInfo.CreatedOn,
                                            CreditCardNo ?? null,
                                            DebitCardNo ?? null);
    }
}