using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.CreditCardApiRequests
{
    public class ValidateCreditCardPINRequest
    {
        public string CreditCardNo { get; set; }

        public string PIN { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ValidateCreditCardPINRequest(string creditCardNo, string pin, CreationInfo creationInfo)
        {
            CreditCardNo = Throw.ArgumentNullException.IfNull(creditCardNo, nameof(creditCardNo));
            PIN = Throw.ArgumentNullException.IfNull(pin, nameof(pin));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
