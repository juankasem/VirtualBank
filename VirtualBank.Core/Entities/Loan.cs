using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
    public class Loan : BaseClass
    {
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey(nameof(BankAccount))]
        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        [Required]
        public LoanType LoanType { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal  InterestRate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}
