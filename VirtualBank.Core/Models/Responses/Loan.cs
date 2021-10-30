using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models.Responses
{
    public class Loan
    {
        public string Id { get; set; }

        public BankAccountCustomer BankAccountCustomer { get; set; }

        public LoanType LoanType { get; set; }

        public Money Amount { get; set; }

        public Amount InterestRate { get; set; }

        public DateTime DueDate { get; set; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public Loan(string id, BankAccountCustomer bankAccountCustomer, LoanType loanType,
                    Money amount, Amount interestRate, DateTime dueDate,
                    CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            BankAccountCustomer = Throw.ArgumentNullException.IfNull(bankAccountCustomer, nameof(bankAccountCustomer));
            LoanType = Throw.ArgumentNullException.IfNull(loanType, nameof(loanType));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            InterestRate = Throw.ArgumentNullException.IfNull(interestRate, nameof(interestRate));
            DueDate = Throw.ArgumentNullException.IfNull(dueDate, nameof(dueDate));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}