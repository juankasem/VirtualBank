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

        public BankAccountOwner Owner { get; set; }

        public BankAccountBranch Branch { get; set; }

        public Amount Balance { get; set; }

        public Amount AllowedBalanceToUse { get; set; }

        public Amount MinimumAllowedBalance { get; set; }

        public Amount Debt { get; set; }

        public BankAccountCurrency Currency { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }

        public bool Disabled { get; set; }

        public DateTime? LastTransactionDate { get; set; }



        public BankAccount(int id, string accountNo, string iban, AccountType type, BankAccountOwner owner,
                           BankAccountBranch branch, Amount balance, Amount allowedBalanceToUse, Amount minimumAllowedBalance,
                           Amount debt, BankAccountCurrency currency, CreationInfo creationInfo,
                           ModificationInfo modificationInfo, bool disabled, DateTime? lastTransactionDate = null)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            Owner = Throw.ArgumentNullException.IfNull(owner, nameof(owner));
            Branch = Throw.ArgumentNullException.IfNull(branch, nameof(branch));
            Balance = balance;
            AllowedBalanceToUse = allowedBalanceToUse;
            MinimumAllowedBalance = minimumAllowedBalance;
            Debt = debt;
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
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
                                          Branch.Id,
                                          Balance.Value,
                                          AllowedBalanceToUse.Value,
                                          MinimumAllowedBalance.Value,
                                          Debt.Value,
                                          Currency.Id,
                                          CreationInfo.CreatedBy,
                                          CreationInfo.CreatedOn,
                                          ModificationInfo.ModifiedBy,
                                          ModificationInfo.LastModifiedOn,
                                          Disabled);
    }
}
