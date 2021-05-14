using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiRequestModels.AccountApiRequests
{
    public class CreateBankAccountRequest
    {
        public string AccountNo { get; set; }

        public string IBAN { get; set; }

        public AccountType Type { get; set; }

        public int CustomerId { get; set; }
      
        public int BranchId { get; set; }

        public int CurrencyId { get; set; }

        public decimal Balance { get; set; }

        public decimal AllowedBalanceToUse { get; set; }

        public decimal MinimumAllowedBalance { get; set; } = 1;


        //constructor method
        public CreateBankAccountRequest(string accountNo, string iban, AccountType type, int customerId, int branchId, int currencyId,
                                        decimal balance, decimal allowedBalanceToUse, decimal minimumAllowedBalance)
        {
            AccountNo = Throw.ArgumentNullException.IfNull(accountNo, nameof(accountNo));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            CustomerId = Throw.ArgumentNullException.IfNull(customerId, nameof(customerId));
            BranchId = Throw.ArgumentNullException.IfNull(branchId, nameof(branchId));
            CurrencyId = Throw.ArgumentNullException.IfNull(currencyId, nameof(currencyId));
            Balance = Throw.ArgumentNullException.IfNull(balance, nameof(balance));
            AllowedBalanceToUse = Throw.ArgumentNullException.IfNull(allowedBalanceToUse, nameof(allowedBalanceToUse));
            MinimumAllowedBalance = Throw.ArgumentNullException.IfNull(minimumAllowedBalance, nameof(minimumAllowedBalance));
        }
    }
}
