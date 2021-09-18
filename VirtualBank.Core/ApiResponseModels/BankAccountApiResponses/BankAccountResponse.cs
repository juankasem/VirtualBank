using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiResponseModels.BankAccountApiResponses
{
    public class BankAccountResponse
    {
        public int Id { get; }

        public string AccountNo { get; }

        public string IBAN { get; }

        public AccountType Type { get; }

        public string AccountOwner { get; }

        public string BranchCode { get; }

        public string BranchName { get; }

        public Amount Balance { get; }

        public Amount AllowedBalanceToUse { get; }

        public Amount Debt { get; }

        public Currency Currency { get; }

        public DateTime CreatedOn { get; }

        public DateTime? LastTransactionDate { get; }

        public BankAccountResponse(int id, string accountNo, string iban, AccountType type, string accountOwner,
                                   string branchCode, string branchName, Amount balance, Amount allowedBalanceToUse,
                                   Amount debt, Currency currency, DateTime createdOn, DateTime? lastTransactionDate = null)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            AccountOwner = Throw.ArgumentNullException.IfNull(accountOwner, nameof(accountOwner));
            BranchCode = Throw.ArgumentNullException.IfNull(branchCode, nameof(branchCode));
            BranchName = Throw.ArgumentNullException.IfNull(branchName, nameof(branchName));
            Balance = balance;
            AllowedBalanceToUse = allowedBalanceToUse;
            Debt = debt;
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            LastTransactionDate = lastTransactionDate;
        }
    }
}
