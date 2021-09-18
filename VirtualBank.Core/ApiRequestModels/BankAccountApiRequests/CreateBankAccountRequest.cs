using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.BankAccountApiRequests
{
    public class CreateBankAccountRequest
    {
        public string AccountNo { get; set; }

        public string IBAN { get; set; }

        public AccountType Type { get; set; }

        public int CustomerId { get; set; }

        public int BranchId { get; set; }

        public int CurrencyId { get; set; }

        public Amount Balance { get; set; }

        public Amount AllowedBalanceToUse { get; set; }

        public Amount MinimumAllowedBalance { get; set; }

        public Amount Debt { get; set; } = new Amount(0);

        public CreationInfo CreationInfo { get; set; }


        //constructor method
        public CreateBankAccountRequest(string accountNo, string iban, AccountType type, int customerId, int branchId,
                                        int currencyId, Amount balance, Amount allowedBalanceToUse, CreationInfo creationInfo)
        {
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            CustomerId = Throw.ArgumentNullException.IfNull(customerId, nameof(customerId));
            BranchId = Throw.ArgumentNullException.IfNull(branchId, nameof(branchId));
            CurrencyId = Throw.ArgumentNullException.IfNull(currencyId, nameof(currencyId));
            Balance = balance;
            AllowedBalanceToUse = allowedBalanceToUse;
            MinimumAllowedBalance = new Amount(1);
            Debt = new Amount(0);
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
