using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.UtilityPaymentApiResponses
{
    public class UtilityPaymentResponse
    {
        public string Id { get; }

        public string BankAccount { get;  }

        public string CustomerName { get; }

        public UtilityType Type { get; }

        public string SubscriptionNo { get; }

        public string InvoiceNo { get; }

        public string CompanyName { get; }

        public decimal Amount { get; }

        public DateTime PaymentDate { get;  }


        public UtilityPaymentResponse(string id, string bankAccountNo, string customerName, UtilityType type, string subscriptionNo,
                                      string invoiceNo, string companyName, decimal amount, DateTime paymentDate)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            BankAccount = Throw.ArgumentNullException.IfNull(bankAccountNo, nameof(bankAccountNo));
            CustomerName = Throw.ArgumentNullException.IfNull(customerName, nameof(customerName));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            SubscriptionNo = Throw.ArgumentNullException.IfNull(subscriptionNo, nameof(subscriptionNo));
            InvoiceNo = Throw.ArgumentNullException.IfNull(invoiceNo, nameof(invoiceNo));
            CompanyName = Throw.ArgumentNullException.IfNull(companyName, nameof(companyName));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            PaymentDate = Throw.ArgumentNullException.IfNull(paymentDate, nameof(paymentDate));
        }
    }
}
