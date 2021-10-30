using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class Loan
    {
        public Guid Id { get; set; }

        public BankAccountCustomer BankAccountCustomer { get; set; }

        public LoanType LoanType { get; set; }

        public Money Amount { get; set; }

        public Amount InterestRate { get; set; }

        public DateTime DueDate { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }


        public Loan(Guid id, BankAccountCustomer bankAccountCustomer, LoanType loanType,
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

        public Core.Entities.Loan ToEntity() =>
                new Core.Entities.Loan(Id,
                                    BankAccountCustomer.CustomerId,
                                    BankAccountCustomer.IBAN,
                                    LoanType,
                                    Amount.Amount.Value,
                                    InterestRate.Value,
                                    DueDate,
                                    CreationInfo.CreatedBy,
                                    CreationInfo.CreatedOn,
                                    ModificationInfo.ModifiedBy,
                                    ModificationInfo.LastModifiedOn);
    }
}