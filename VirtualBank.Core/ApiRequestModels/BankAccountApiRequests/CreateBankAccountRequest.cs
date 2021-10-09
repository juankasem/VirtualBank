using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.BankAccountApiRequests
{
    public class CreateBankAccountRequest
    {
        public string AccountNo { get; set; }

        public string IBAN { get; set; }

        public AccountType Type { get; set; }

        public BankAccountOwner Owner { get; set; }

        public BankAccountBranch Branch { get; set; }

        public BankAccountCurrency Currency { get; set; }

        public Amount Balance { get; set; }

        public Amount AllowedBalanceToUse { get; set; }

        public Amount MinimumAllowedBalance { get; set; }

        public Amount Debt { get; set; } = new Amount(0);

        public CreationInfo CreationInfo { get; set; }


        public CreateBankAccountRequest(string accountNo, string iban, AccountType type,
                                        BankAccountOwner owner, BankAccountBranch branch,
                                        BankAccountCurrency currency, Amount balance, Amount allowedBalanceToUse,
                                        CreationInfo creationInfo)
        {
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            Owner = Throw.ArgumentNullException.IfNull(owner, nameof(owner));
            Branch = Throw.ArgumentNullException.IfNull(branch, nameof(branch));
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
            Balance = balance;
            AllowedBalanceToUse = allowedBalanceToUse;
            MinimumAllowedBalance = new Amount(1);
            Debt = new Amount(0);
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
