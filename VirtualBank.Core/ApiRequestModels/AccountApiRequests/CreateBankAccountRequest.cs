using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiRequestModels.AccountApiRequests
{
    public class CreateBankAccountRequest
    {
        public BankAccount Account { get; set; }

        public CreateBankAccountRequest(BankAccount account)
        {
            Account = Throw.ArgumentNullException.IfNull(account, nameof(account));
        }
    }
}
