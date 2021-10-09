using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models.Responses
{
    public class BankAccount
    {
        public int Id { get; }

        public string AccountNo { get; set; }

        public string IBAN { get; set; }

        public AccountType Type { get; set; }

        public BankAccountOwner Owner { get; set; }

        public BankAccountBranch Branch { get; set; }

        public Amount Balance { get; set; }

        public Amount AllowedBalanceToUse { get; set; }

        public Amount Debt { get; set; }

        public BankAccountCurrency Currency { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }

        public DateTime? LastTransactionDate { get; set; }


        public BankAccount(int id, string accountNo, string iban, AccountType type, BankAccountOwner owner,
                           BankAccountBranch branch, Amount balance, Amount allowedBalanceToUse,
                           Amount debt, BankAccountCurrency currency, CreationInfo creationInfo, ModificationInfo modificationInfo,
                           DateTime? lastTransactionDate)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            Owner = Throw.ArgumentNullException.IfNull(owner, nameof(owner));
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