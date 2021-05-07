using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.AccountApiResponses
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

        public decimal Balance { get; }

        public decimal AllowedBalanceToUse { get; }

        public string Currency { get; }

        public DateTime CreatedOn { get; }

        public DateTime? LastTransactionDate { get; }

        public BankAccountResponse(int id, string accountNo, string iban, AccountType type, string accountOwner,
                                   string branchCode, string branchName, decimal balance, decimal allowedBalanceToUse,
                                   string currency, DateTime createdOn, DateTime? lastTransactionDate = null)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            AccountOwner = Throw.ArgumentNullException.IfNull(accountOwner, nameof(accountOwner));
            BranchCode = Throw.ArgumentNullException.IfNull(branchCode, nameof(branchCode));
            BranchName = Throw.ArgumentNullException.IfNull(branchName, nameof(branchName));
            Balance = Throw.ArgumentNullException.IfNull(balance, nameof(balance));
            AllowedBalanceToUse = Throw.ArgumentNullException.IfNull(allowedBalanceToUse, nameof(allowedBalanceToUse));
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            LastTransactionDate = lastTransactionDate;
        }
    }
}
