using System;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.Domain.Models
{
    public class UtilityPayment
    {
        public Guid Id { get; }

        public UtilityType Type { get; }

        public string IBAN { get; }

        public string SubscriptionNo { get; }

        public string InvoiceNo { get; }

        public string CompanyName { get; }

        public Money Amount { get; }

        public DateTime PaymentDate { get; }

        public CreationInfo CreationInfo { get; }

        public ModificationInfo ModificationInfo { get; }


        public UtilityPayment(Guid id, UtilityType type, string iban, string subscriptionNo, string invoiceNo,
                              string companyName, Money amount, DateTime paymentDate,
                              CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            Id = id;
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            SubscriptionNo = Throw.ArgumentNullException.IfNull(subscriptionNo, nameof(subscriptionNo));
            InvoiceNo = Throw.ArgumentNullException.IfNull(invoiceNo, nameof(invoiceNo));
            CompanyName = Throw.ArgumentNullException.IfNull(companyName, nameof(companyName));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            PaymentDate = Throw.ArgumentNullException.IfNull(paymentDate, nameof(paymentDate));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }

        public Core.Entities.UtilityPayment ToEntity() =>
            new Core.Entities.UtilityPayment(Id,
                                             Type,
                                             IBAN,
                                             SubscriptionNo,
                                             InvoiceNo,
                                             CompanyName,
                                             Amount.Amount.Value,
                                             Amount.Currency.Id,
                                             PaymentDate,
                                             CreationInfo.CreatedBy,
                                             CreationInfo.CreatedOn,
                                             ModificationInfo.ModifiedBy,
                                             ModificationInfo.LastModifiedOn);
    }
}