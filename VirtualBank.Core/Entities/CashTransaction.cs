using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
    public class CashTransaction : BaseClass
    {
        [Required]
        public CashTransactionType Type { get; set; }

        [Required]
        public BankAssetType InitiatedBy { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        [Column(TypeName="decimal(8,2)")]
        public decimal Amount { get; set; }

        [Required]
        public Currency Currency { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal RemainingBalance { get; set; }

        [Required]
        public double TransferFees { get; set; }

        public string Description { get; set; }

        [Required]
        public PaymentType PaymentType { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public TransactionStatusType Status { get; set; }
    }
}
