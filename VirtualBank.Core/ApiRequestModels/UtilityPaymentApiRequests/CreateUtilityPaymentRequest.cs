﻿using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiRequestModels.UtilityPaymentApiRequests
{
    public class CreateUtilityPaymentRequest
    {
        public string IBAN { get; set; }

        public UtilityType Type { get; set; }

        public string SubscriptionNo { get; set; }

        public string InvoiceNo { get; set; }

        public string CompanyName { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }


        public CreateUtilityPaymentRequest(string iban, UtilityType type, string subscriptionNo,
                                           string invoiceNo, string companyName, decimal amount, DateTime paymentDate)
        {
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            SubscriptionNo = Throw.ArgumentNullException.IfNull(subscriptionNo, nameof(subscriptionNo));
            InvoiceNo = Throw.ArgumentNullException.IfNull(invoiceNo, nameof(invoiceNo));
            CompanyName = Throw.ArgumentNullException.IfNull(companyName, nameof(companyName));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            PaymentDate = Throw.ArgumentNullException.IfNull(paymentDate, nameof(paymentDate));
        }
    }
}
