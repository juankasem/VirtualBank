using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
    public class Account : BaseClass
    {
        [Required]
        [MaxLength(50)]
        public string AccountNo { get; set; }

        [Required]
        [MaxLength(50)]
        public string IBAN { get; set; }

        [Required]
        public AccountType Type { get; set; }

        [ForeignKey("Owner")]
        [Required]
        public string CustomerId { get; set; }
        public Customer Owner { get; set; }

        [ForeignKey("Branch")]
        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Balance { get; set; }

        [Required]
        public Currency Currency { get; set; }

        public ICollection<CreditCard> CreditCards { get; set; }

    }
}
