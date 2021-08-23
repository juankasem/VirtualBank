using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.DebitCardApiRequests
{
    public class CreateDebitCardRequest
    {
        public string DebitCardNo { get; set; }

        public string PIN { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int BankAccountId { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public CreateDebitCardRequest(string debitCardNo, string pin, DateTime expirationDate, int bankAccountId, CreationInfo creationInfo)
        {
            DebitCardNo = Throw.ArgumentNullException.IfNull(debitCardNo, nameof(debitCardNo));
            PIN = Throw.ArgumentNullException.IfNull(pin, nameof(pin));
            ExpirationDate = Throw.ArgumentOutOfRangeException.IfLessThan(expirationDate, DateTime.Now, nameof(expirationDate));
            BankAccountId = Throw.ArgumentNullException.IfNull(bankAccountId, nameof(bankAccountId));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
