using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiResponseModels.AccountApiResponses
{
    public class AccountResponse
    {
        public Account Account { get; }

        public AccountResponse(Account account)
        {
            Account = Throw.ArgumentNullException.IfNull(account, nameof(account));
        }
    }
}
