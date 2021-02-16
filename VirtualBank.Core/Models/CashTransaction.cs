using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models
{
    public class CashTransaction
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public CashTransactionType Type { get; set; }

        [Required]
        public BankAsset From { get; set; }

        [Required]
        public BankAsset To { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime CreatedBy { get; set; }

        public TransactionStatusType Status { get; set; }
    }
}
