using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.DebitCardApiRequests
{
    public class CreateDebitCardRequest
    {
        public string DebitCardNo { get; set; }

        public string PIN { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int BankAccountId { get; set; }


        public CreateDebitCardRequest(string debitCardNo, string pin, DateTime expirationDate, int bankAccountId)
        {
            DebitCardNo = Throw.ArgumentNullException.IfNull(debitCardNo, nameof(debitCardNo));
            PIN = Throw.ArgumentNullException.IfNull(pin, nameof(pin));
            ExpirationDate = Throw.ArgumentOutOfRangeException.IfLessThan(expirationDate, DateTime.Now, nameof(expirationDate));
            BankAccountId = Throw.ArgumentNullException.IfNull(bankAccountId, nameof(bankAccountId));
        }
    }
}
