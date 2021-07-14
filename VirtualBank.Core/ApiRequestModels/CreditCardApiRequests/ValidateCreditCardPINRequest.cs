using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.CreditCardApiRequests
{
    public class ValidateCreditCardPINRequest
    {
        public string CreditCardNo { get; set; }

        public string PIN { get; set; }

        public ValidateCreditCardPINRequest(string creditCardNo, string pin)
        {
            CreditCardNo = Throw.ArgumentNullException.IfNull(creditCardNo, nameof(creditCardNo));
            PIN = Throw.ArgumentNullException.IfNull(pin, nameof(pin));
        }
    }
}
