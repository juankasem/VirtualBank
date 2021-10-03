using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models.Responses
{
    public class BankAccount
    {
        public int Id { get; }

        public string AccountNo { get; set; }

        public string IBAN { get; set; }

        public AccountType Type { get; set; }

        public string AccountOwner { get; set; }

        public Branch AccountBranch { get; set; }

        public Amount Balance { get; set; }

        public Amount AllowedBalanceToUse { get; set; }

        public Amount Debt { get; set; }

        public Currency AccountCurrency { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }

        public DateTime? LastTransactionDate { get; set; }


        public BankAccount(int id, string accountNo, string iban, AccountType type, string accountOwner,
                           Branch accountBranch, Amount balance, Amount allowedBalanceToUse,
                           Amount debt, Currency accountCurrency, CreationInfo creationInfo, ModificationInfo modificationInfo,
                           DateTime? lastTransactionDate)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            AccountOwner = Throw.ArgumentNullException.IfNull(accountOwner, nameof(accountOwner));
            AccountBranch = Throw.ArgumentNullException.IfNull(accountBranch, nameof(accountBranch));
            Balance = balance;
            AllowedBalanceToUse = allowedBalanceToUse;
            Debt = debt;
            AccountCurrency = Throw.ArgumentNullException.IfNull(accountCurrency, nameof(accountCurrency));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
            LastTransactionDate = lastTransactionDate;
        }

        public class Branch
        {
            public int Id { get; }

            public string Name { get; }

            public string Code { get; }

            public Branch(int id, string name, string code)
            {
                Id = id;
                Name = Throw.ArgumentNullException.IfNull(name, nameof(name));
                Code = Throw.ArgumentNullException.IfNull(code, nameof(code));
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