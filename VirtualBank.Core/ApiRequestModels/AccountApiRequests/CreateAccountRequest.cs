using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiRequestModels.AccountApiRequests
{
    public class CreateAccountRequest
    {
        public Account Account { get; }

        public CreateAccountRequest(Account account)
        {
            Account = Throw.ArgumentNullException.IfNull(account, nameof(account));
        }
    }
}
