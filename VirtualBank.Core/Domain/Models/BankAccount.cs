using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class BankAccount
    {
        public int Id { get; }

        public string AccountNo { get; set; }

        public string IBAN { get; set; }

        public AccountType Type { get; set; }

        public AccountOwner Owner { get; set; }

        public Branch AccountBranch { get; set; }

        public Amount Balance { get; set; }

        public Amount AllowedBalanceToUse { get; set; }

        public Amount MinimumAllowedBalance { get; set; }

        public Amount Debt { get; set; }

        public Currency AccountCurrency { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }

        public bool Disabled { get; set; }

        public DateTime? LastTransactionDate { get; set; }



        public BankAccount(int id, string accountNo, string iban, AccountType type, AccountOwner owner,
                           Branch accountBranch, Amount balance, Amount allowedBalanceToUse, Amount minimumAllowedBalance,
                           Amount debt, Currency accountCurrency, CreationInfo creationInfo, ModificationInfo modificationInfo,
                           bool disabled, DateTime? lastTransactionDate = null)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            Owner = Throw.ArgumentNullException.IfNull(owner, nameof(owner));
            AccountBranch = Throw.ArgumentNullException.IfNull(accountBranch, nameof(accountBranch));
            Balance = balance;
            AllowedBalanceToUse = allowedBalanceToUse;
            MinimumAllowedBalance = minimumAllowedBalance;
            Debt = debt;
            AccountCurrency = Throw.ArgumentNullException.IfNull(accountCurrency, nameof(accountCurrency));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
            Disabled = disabled;
            LastTransactionDate = lastTransactionDate;
        }

        public Core.Entities.BankAccount ToEntity() =>
            new Core.Entities.BankAccount(
                                          AccountNo,
                                          IBAN,
                                          Type,
                                          Owner.CustomerId,
                                          AccountBranch.Id,
                                          Balance.Value,
                                          AllowedBalanceToUse.Value,
                                          MinimumAllowedBalance.Value,
                                          Debt.Value,
                                          AccountCurrency.Id,
                                          CreationInfo.CreatedBy,
                                          CreationInfo.CreatedOn,
                                          ModificationInfo.ModifiedBy,
                                          ModificationInfo.LastModifiedOn,
                                          Disabled);

        public class AccountOwner
        {
            public int CustomerId { get; }

            public string FullName { get; }


            public AccountOwner(int customerId, string fullName)
            {
                CustomerId = Throw.ArgumentNullException.IfNull(customerId, nameof(customerId));
                FullName = Throw.ArgumentNullException.IfNull(fullName, nameof(fullName));
            }
        }

        public class Branch
        {
            public int Id { get; }

            public string Code { get; }

            public string Name { get; }

            public string City { get; }


            public Branch(int id, string code, string name, string city)
            {
                Id = id;
                Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
                Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
                City = Throw.ArgumentNullException.IfNull(city, nameof(city));
            }
        }

        public class Currency
        {
            public int Id { get; }

            public string Code { get; }

            public string Symbol { get; }


            public Currency(int id, string code, string symbol)
            {
                Id = id;
                Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
                Symbol = Throw.ArgumentNullException.IfNull(symbol, nameof(symbol));
            }
        }
    }
}
