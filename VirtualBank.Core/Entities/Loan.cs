using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Entities
{
    public class Loan : BaseGUIDClass
    {

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey(nameof(BankAccount))]
        public string IBAN { get; set; }
        public BankAccount BankAccount { get; set; }

        [Required]
        public LoanType LoanType { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal InterestRate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }


        public Loan(Guid id, int customerId, string iban, LoanType loanType, decimal amount,
                    decimal interestRate, DateTime dueDate, string createdBy, DateTime createdOn,
                    string modifiedBy, DateTime lastModifiedOn)
        {
            Id = id;
            CustomerId = customerId;
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            LoanType = Throw.ArgumentNullException.IfNull(loanType, nameof(loanType));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            InterestRate = Throw.ArgumentOutOfRangeException.IfLessThan(interestRate, 0, nameof(interestRate));
            DueDate = Throw.ArgumentNullException.IfNull(dueDate, nameof(dueDate));
            CreatedBy = Throw.ArgumentNullException.IfNull(createdBy, nameof(createdBy));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            LastModifiedBy = Throw.ArgumentNullException.IfNull(modifiedBy, nameof(modifiedBy));
            LastModifiedOn = Throw.ArgumentNullException.IfNull(lastModifiedOn, nameof(lastModifiedOn));
        }

        public Domain.Models.Loan ToDomainModel() =>
                 new Domain.Models.Loan(Id,
                                        CreateBankAccountCustomer(CustomerId, CreateCustomerName(Customer.FirstName, Customer.LastName), IBAN),
                                        LoanType,
                                        CreateMoney(Amount, BankAccount.Currency.Code),
                                        new Amount(InterestRate),
                                        DueDate,
                                        CreateCreationInfo(CreatedBy, CreatedOn),
                                        CreateModificationInfo(LastModifiedBy, LastModifiedOn)
                                        );


        private BankAccountCustomer CreateBankAccountCustomer(int customerId, string customerName, string iban) =>
             new BankAccountCustomer(customerId, customerName, iban);

        private string CreateCustomerName(string firstName, string lastName) =>
             firstName + " " + lastName;

        private Money CreateMoney(decimal amount, string currency) =>
            new Money(new Amount(amount), currency);

        private CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) =>
           new CreationInfo(createdBy, createdOn);

        private ModificationInfo CreateModificationInfo(string lastModifiedBy, DateTime lastModifiedOn) =>
           new ModificationInfo(lastModifiedBy, lastModifiedOn);
    }
}
