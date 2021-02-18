﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels
{
    public class CashTransactionRequest
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public CashTransactionType Type { get; set; }

        [Required]
        public BankAsset From { get; set; }

        [Required]
        public BankAsset To { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public AppUser CreatedBy { get; set; }

        [Required]
        public TransactionStatusType Status { get; set; }
    }
}
