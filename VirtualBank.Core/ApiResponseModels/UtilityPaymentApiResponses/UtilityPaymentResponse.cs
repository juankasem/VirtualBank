using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.UtilityPaymentApiResponses
{
    public class UtilityPaymentResponse
    {
        public UtilityPayment UtilityPayment { get; }


        public UtilityPaymentResponse(UtilityPayment utilityPayment)
        {
            UtilityPayment = Throw.ArgumentNullException.IfNull(utilityPayment, nameof(utilityPayment));
        }
    }
}
