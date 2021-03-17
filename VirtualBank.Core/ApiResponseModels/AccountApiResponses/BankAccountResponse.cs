using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.AccountApiResponses
{
    public class BankAccountResponse
    {
        public int Id { get; set; }

        public string AccountNo { get; set; }

        public string IBAN { get; set; }

        public AccountType Type { get; set; }

        public string AccountOwner { get; set; }

        public string BranchCode { get; set; }

        public string BranchName { get; set; }

        public decimal Balance { get; set; }

        public decimal AllowedBalanceToUse { get; set; }

        public string Currency { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastTransactionDate { get; set; }

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
