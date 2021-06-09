using System;
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

        public LoanType LoanType { get; set; }

        public decimal Amount { get; set; }

        public decimal  InterestRate { get; set; }

        public DateTime DueDate { get; set; }
    }
}
