using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class RecipientBankAccount
    {
        public string AccountNo { get; }

        public string IBAN { get; }

        public string AccountOwner { get; }

        public AccountType Type { get; }

        public string BranchName { get; }

        public string BranchCity { get; }

        public string Currency { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public RecipientBankAccount(string accountNo, string iban, string accountOwner, AccountType type,
                                    string branchName, string branchCity, string currency,
                                    CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            AccountOwner = Throw.ArgumentNullException.IfNull(accountOwner, nameof(accountOwner));
            BranchName = Throw.ArgumentNullException.IfNull(branchName, nameof(branchName));
            BranchCity = Throw.ArgumentNullException.IfNull(branchCity, nameof(branchCity));
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}