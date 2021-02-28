using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    public class SavingsAccount : BankAccount
    {
        [Required]
        [Column(TypeName = "decimal(4,2)")]
        public double InterestRate { get; set; }

        [Required]
        public int AllowedNumOfTransactions { get; set; }

        [Required]
        public DateTime BlockPeriodTill { get; set; }
    }
}
