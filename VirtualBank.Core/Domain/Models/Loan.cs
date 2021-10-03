using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class Loan
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string IBAN { get; set; }

        public LoanType LoanType { get; set; }

        public decimal Amount { get; set; }

        public decimal InterestRate { get; set; }

        public DateTime DueDate { get; set; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public Loan(int id, string customerName, string iban, LoanType loanType,
                    decimal amount, decimal interestRate, DateTime dueDate,
                    CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            CustomerName = Throw.ArgumentNullException.IfNull(customerName, nameof(customerName));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            LoanType = Throw.ArgumentNullException.IfNull(loanType, nameof(loanType));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            InterestRate = Throw.ArgumentNullException.IfNull(interestRate, nameof(interestRate));
            DueDate = Throw.ArgumentNullException.IfNull(dueDate, nameof(dueDate));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}