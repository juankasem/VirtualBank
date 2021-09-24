using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models.Responses
{
    public class BankAccount
    {
        public int Id { get; }

        public string AccountNo { get; }

        public string IBAN { get; }

        public AccountType Type { get; }

        public string AccountOwner { get; }

        public Branch Branch { get; }

        public Amount Balance { get; }

        public Amount AllowedBalanceToUse { get; }

        public Amount Debt { get; }

        public Currency Currency { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }

        public DateTime? LastTransactionDate { get; }

        public BankAccount(int id, string accountNo, string iban, AccountType type,
                           string accountOwner, Branch branch, Amount balance, Amount allowedBalanceToUse,
                            Amount debt, Currency currency, CreationInfo creationInfo, ModificationInfo modificationInfo,
                            DateTime? lastTransactionDate = null)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            AccountOwner = Throw.ArgumentNullException.IfNull(accountOwner, nameof(accountOwner));
            Branch = Throw.ArgumentNullException.IfNull(branch, nameof(branch));
            Balance = balance;
            AllowedBalanceToUse = allowedBalanceToUse;
            Debt = debt;
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
            LastTransactionDate = lastTransactionDate;
        }
    }
}