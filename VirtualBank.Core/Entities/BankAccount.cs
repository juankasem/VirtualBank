using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.ArgumentChecks;
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
        public decimal Balance { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal AllowedBalanceToUse { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal MinimumAllowedBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Debt { get; set; }

        [ForeignKey(nameof(Currency))]
        [Required]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public ICollection<CashTransaction> CashTransactions { get; set; }

        public ICollection<CreditCard> CreditCards { get; set; }

        public BankAccount(string accountNo, string iban, AccountType type, int customerId, int branchId,
                           decimal balance, decimal allowedBalanceToUse, decimal minimumAllowedBalance, decimal debt, int currencyId,
                           string createdBy, DateTime createdOn, string lastModifiedBy, DateTime lastModifiedOn, bool disabled)
        {
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            CustomerId = customerId;
            BranchId = branchId;
            Balance = Throw.ArgumentOutOfRangeException.IfLessThan(balance, 0, nameof(balance));
            AllowedBalanceToUse = Throw.ArgumentOutOfRangeException.IfLessThan(allowedBalanceToUse, 0, nameof(allowedBalanceToUse));
            MinimumAllowedBalance = minimumAllowedBalance;
            Debt = debt;
            CurrencyId = currencyId;
            CreatedBy = Throw.ArgumentNullException.IfNull(createdBy, nameof(createdBy));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            LastModifiedBy = Throw.ArgumentNullException.IfNull(lastModifiedBy, nameof(lastModifiedBy));
            LastModifiedOn = Throw.ArgumentNullException.IfNull(lastModifiedOn, nameof(lastModifiedOn));
            Disabled = disabled;
        }

        public Core.Domain.Models.BankAccount ToDomainModel() =>
           new Core.Domain.Models.BankAccount(Id,
                                              AccountNo,
                                              IBAN,
                                              Type,
                                              CreateAccountOwner(CustomerId, Owner.FirstName, Owner.LastName, Owner.Gender),
                                              CreateAccountBranch(Branch),
                                              new Amount(Balance),
                                              new Amount(AllowedBalanceToUse),
                                              new Amount(MinimumAllowedBalance),
                                              new Amount(Debt),
                                              CreateAccountCurrency(Currency),
                                              new CreationInfo(CreatedBy, CreatedOn),
                                              new ModificationInfo(LastModifiedBy, LastModifiedOn),
                                              Disabled,
                                              CashTransactions.LastOrDefault()?.CreatedOn);

        private Domain.Models.BankAccountOwner CreateAccountOwner(int customerId, string firstName, string lastName, Gender gender) =>
             new Domain.Models.BankAccountOwner(customerId, CreateOwnerName(firstName, lastName), gender);


        private string CreateOwnerName(string firstName, string lastName) =>
           firstName + " " + lastName;

        private Domain.Models.BankAccountBranch CreateAccountBranch(Branch branch) =>
            new Domain.Models.BankAccountBranch(branch.Id, branch.Code, branch.Name, branch.Address.City.Name);

        private Domain.Models.BankAccountCurrency CreateAccountCurrency(Currency currency) =>
            new Domain.Models.BankAccountCurrency(currency.Id, currency.Code, currency.Symbol);
    }
}

