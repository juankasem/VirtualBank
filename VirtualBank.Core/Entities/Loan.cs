using System;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
    public class Loan : BaseClass
    {
        public string CustomerId { get; set; }

        public LoanType LoanType { get; set; }

        public decimal Amount { get; set; }

        public decimal  InterestRate { get; set; }

        public DateTime DueDate { get; set; }

    }
}
