using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models
{
    public class CashTransaction : BaseData
    {
        [Required]
        public CashTransactionType Type { get; set; }

        [Required]
        public BankAsset From { get; set; }

        [Required]
        public BankAsset To { get; set; }

        [Required]
        [Column(TypeName="decimal(8,2)")]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        [Required]
        public TransactionStatusType Status { get; set; }
    }
}
