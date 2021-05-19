using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.CreditCardApiRequests
{
    public class CreateCreditCardRequest
    {
        public string CreditCardNo { get; set; }

        public string PIN { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int BankAccountId { get; set; }


        public CreateCreditCardRequest(string creditCardNo, string pin, DateTime expirationDate, int bankAccountId)
        {
            CreditCardNo = Throw.ArgumentNullException.IfNull(creditCardNo, nameof(creditCardNo));
            PIN = Throw.ArgumentNullException.IfNull(pin, nameof(pin));
            ExpirationDate = Throw.ArgumentOutOfRangeException.IfLessThan(expirationDate, DateTime.Now, nameof(expirationDate));
            BankAccountId = Throw.ArgumentNullException.IfNull(bankAccountId, nameof(bankAccountId));
        }
    }
}
