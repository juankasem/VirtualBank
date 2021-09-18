using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Entities
{
    public class BankAccount : BaseClass
    {
        [Required]
        [MaxLength(50)]
        public string AccountNo { get; set; }

        [Required]
        [MaxLength(50)]
        public string IBAN { get; set; }

        [Required]
        public AccountType Type { get; set; }

        [ForeignKey(nameof(Owner))]
        [Required]
        public int CustomerId { get; set; }
        public Customer Owner { get; set; }

        [ForeignKey(nameof(Branch))]
        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public Amount Balance { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public Amount AllowedBalanceToUse { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public Amount MinimumAllowedBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public Amount Debt { get; set; }

        [ForeignKey(nameof(Currency))]
        [Required]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public ICollection<CashTransaction> CashTransactions { get; set; }

        public ICollection<CreditCard> CreditCards { get; set; }

    }
}

