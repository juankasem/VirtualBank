using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models.Responses
{
    public class CashTransaction
    {
        /// <summary>
        /// Id of the transaction
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// reference of transaction
        /// </summary>
        public string ReferenceNo { get; }

        /// <summary>
        /// reference of transaction
        /// </summary>
        public CashTransactionType Type { get; }

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
        public Money RemainingBalance { get; }

        /// <summary>
        /// remaining balance after transaction
        /// </summary>
        public DateTime TransactionDate { get; }

        /// <summary>
        /// creation info the transaction
        /// </summary>
        public CreationInfo CreationInfo { get; }

        /// <summary>
        /// creation info the transaction
        /// </summary>

        public ModificationInfo ModificationInfo { get; }

        /// <summary>
        /// credit card number
        /// </summary>
#nullable enable
        public string? CreditCardNo { get; set; }

        /// <summary>
        /// debit card number
        /// </summary>
#nullable enable
        public string? DebitCardNo { get; set; }


        public CashTransaction(int id, string referenceNo, BankAssetType initiatedBy, string from, string to,
                               string sender, string recipient, Money debitedFunds, Money fees,
                               PaymentType paymentType, string description, Money remainingBalance,
                               DateTime transactionDate, CreationInfo creationInfo, ModificationInfo modificationInfo,
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
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
            CreditCardNo = creditCardNo;
            DebitCardNo = debitCardNo;
        }

        public Core.Entities.CashTransaction ToEntity(decimal senderRemainingBalance, decimal recipientRemainingBalance) =>
         new Core.Entities.CashTransaction(ReferenceNo,
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
                                           ModificationInfo.ModifiedBy,
                                           ModificationInfo.LastModifiedOn,
                                           CreditCardNo ?? null,
                                           DebitCardNo ?? null);
    }
}
