using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Entities
{
    public class CashTransaction
    {
        /// <summary>
        /// unique identifier
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// unique reference number
        /// </summary>
        [Required]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// transaction type
        /// </summary>
        [Required]
        public CashTransactionType Type { get; set; }

        /// <summary>
        /// bank asset such as bank account, POS, ATM
        /// </summary>
        [Required]
        public BankAssetType InitiatedBy { get; set; }

        /// <summary>
        /// IBAN of bank account of the sender
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string From { get; set; }

        /// <summary>
        /// IBAN of bank account of the recipient
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string To { get; set; }

        /// <summary>
        /// amount of credited funds
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// currency of credited funds
        /// </summary>
        [Required]
        [MaxLength(5)]
        public string Currency { get; set; }

        /// <summary>
        /// remaining balance of the sender account
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal SenderRemainingBalance { get; set; }

        /// <summary>
        /// remaining balance of the recipient account
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal RecipientRemainingBalance { get; set; }

        /// <summary>
        /// fees of transaction
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Fees { get; set; }

        /// <summary>
        /// given description of transaction
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// payment type like rent, salary, etc...
        /// </summary>
        [Required]
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// date of transaction
        /// </summary>
        [Required]
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Created By transaction
        /// </summary>
        [Required]
        public string CreatedBy { get; set; }

        /// <summary>
        /// CDate of creation transaction
        /// </summary>
        [Required]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// debit card number
        /// </summary>
#nullable enable
        public string? CreditCardNo { get; set; }

        /// <summary>
        /// debit card number
        /// </summary>
#nullable enable
        public string? DebitCardNo { get; set; }


        public CashTransaction()
        {
            ReferenceNo = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)}";
        }

        public CashTransaction(Guid id, string referenceNo, CashTransactionType type, BankAssetType initiatedBy, string from, string to,
                               decimal amount, string currency, decimal senderRemainingBalance, decimal recipientRemainingBalance,
                               decimal fees, string description, PaymentType paymentType, DateTime transactionDate,
                               string createdBy, DateTime createdOn, string? creditCardNo, string? debitCardNo)
        {
            Id = id;
            ReferenceNo = referenceNo;
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            InitiatedBy = Throw.ArgumentNullException.IfNull(initiatedBy, nameof(initiatedBy));
            From = Throw.ArgumentNullException.IfNull(from, nameof(from));
            To = Throw.ArgumentNullException.IfNull(to, nameof(to));
            Amount = Throw.ArgumentOutOfRangeException.IfLessThan(amount, 0, nameof(amount));
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
            SenderRemainingBalance = Throw.ArgumentOutOfRangeException.IfLessThan(senderRemainingBalance, 0, nameof(senderRemainingBalance));
            RecipientRemainingBalance = Throw.ArgumentOutOfRangeException.IfLessThan(recipientRemainingBalance, 0, nameof(recipientRemainingBalance));
            Fees = Throw.ArgumentOutOfRangeException.IfLessThan(fees, 0, nameof(fees));
            Description = Throw.ArgumentNullException.IfNull(description, nameof(description));
            PaymentType = Throw.ArgumentNullException.IfNull(paymentType, nameof(paymentType));
            TransactionDate = Throw.ArgumentNullException.IfNull(transactionDate, nameof(transactionDate));
            CreatedBy = Throw.ArgumentNullException.IfNull(createdBy, nameof(createdBy));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            CreditCardNo = creditCardNo;
            DebitCardNo = debitCardNo;
        }

        public Core.Domain.Models.CashTransaction ToDomainModel() =>
             new Core.Domain.Models.CashTransaction(Id,
                                                    ReferenceNo,
                                                    Type,
                                                    InitiatedBy,
                                                    From,
                                                    To,
                                                    new Money(new Amount(Amount), Currency),
                                                    new Money(new Amount(Fees), Currency),
                                                    PaymentType,
                                                    Description,
                                                    new Money(new Amount(SenderRemainingBalance), Currency),
                                                    new Money(new Amount(RecipientRemainingBalance), Currency),
                                                    TransactionDate,
                                                    new CreationInfo(CreatedBy, CreatedOn),
                                                    CreditCardNo ?? null,
                                                    DebitCardNo ?? null);
    }
}
