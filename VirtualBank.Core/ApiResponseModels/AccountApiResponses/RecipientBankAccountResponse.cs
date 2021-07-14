using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.AccountApiResponses
{
    public class RecipientBankAccountResponse
    {
        public string AccountNo { get; }

        public string IBAN { get; }

        public string AccountOwner { get; }

        public AccountType AccountType { get; }

        public string BranchName { get; }

        public string BranchCity { get; }

        public string Currency { get; }


        public RecipientBankAccountResponse(string accountNo, string iban,string accountOwner,
                                            AccountType accountType, string branchName, string branchCity, string currency)
        {
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            AccountType = Throw.ArgumentNullException.IfNull(accountType, nameof(accountType));
            AccountOwner = Throw.ArgumentNullException.IfNull(accountOwner, nameof(accountOwner));
            BranchName = Throw.ArgumentNullException.IfNull(branchName, nameof(branchName));
            BranchCity = Throw.ArgumentNullException.IfNull(branchCity, nameof(branchCity));
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
        }
    }
}
