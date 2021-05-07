using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.AccountApiResponses
{
    public class RecipientBankAccountResponse
    {
        public string AccountNo { get; }

        public string IBAN { get; }

        public AccountType AccountType { get; }

        public string AccountOwner { get; }

        public string BranchName { get; }

        public string Currency { get; }


        public RecipientBankAccountResponse(string accountNo, string iban, AccountType accountType,
                                            string accountOwner, string branchName, string currency)
        {
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            AccountType = Throw.ArgumentNullException.IfNull(accountType, nameof(accountType));
            AccountOwner = Throw.ArgumentNullException.IfNull(accountOwner, nameof(accountOwner));
            BranchName = Throw.ArgumentNullException.IfNull(branchName, nameof(branchName));
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
        }
    }
}
