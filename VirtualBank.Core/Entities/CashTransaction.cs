using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
#nullable enable
    public class CashTransaction : BaseClass
    {
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
        /// unique reference number
        /// </summary>
        public string ReferenceNo { get; set; }

        /// <summary>
        /// IBAN of bank account of the sender
        /// </summary>
        [MaxLength(150)]
        public string From { get; set; }

        /// <summary>
        /// IBAN of bank account of the recipient
        /// </summary>
        [MaxLength(150)]
        public string To { get; set; }

        /// <summary>
        /// amount of credited funds
        /// </summary>
        [Required]
        [Column(TypeName="decimal(8,2)")]
        public decimal Amount { get; set; }


        /// <summary>
        /// currency of credited funds
        /// </summary>
        [Required]
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
        /// debit card number
        /// </summary>
        public string? CreditCardNo { get; set; }
        
        /// <summary>
        /// debit card number
        /// </summary>
        public string? DebitCardNo { get; set; }

        public CashTransaction()
        {
            ReferenceNo = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 27)}";
        }
    }
}
