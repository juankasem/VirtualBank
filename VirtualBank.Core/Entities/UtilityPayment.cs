using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
    public class UtilityPayment : BaseClass
    {
        /// <summary>
        /// transaction type
        /// </summary>
        [Required]
        public UtilityType Type { get; set; }

        /// <summary>
        /// BANK ACCOUNT
        /// </summary>
        [ForeignKey(nameof(BankAccount))]
        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        /// <summary>
        /// Subscription Number
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string SubscriptionNo { get; set; }

        /// <summary>
        /// Invoice Number
        /// </summary>
        [Required]
        public string InvoiceNo { get; set; }

        /// <summary>
        /// company name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        /// <summary>
        /// amount of money
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// date of transaction
        /// </summary>
        [Required]
        public DateTime PaymentDate { get; set; }


        public UtilityPayment()
        {
        }
    }
}
