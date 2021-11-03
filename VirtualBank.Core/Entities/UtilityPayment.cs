using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Entities
{
    public class UtilityPayment : BaseGUIDClass
    {
        /// <summary>
        /// transaction type
        /// </summary>
        [Required]
        public UtilityType Type { get; set; }

        /// <summary>
        /// BANK ACCOUNT
        /// </summary>
        [ForeignKey(nameof(BankAccount))]
        public string IBAN { get; set; }
        public BankAccount BankAccount { get; set; }

        /// <summary>
        /// Subscription Number
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string SubscriptionNo { get; set; }

        /// <summary>
        /// Invoice Number
        /// </summary>
        [Required]
        public string InvoiceNo { get; set; }

        /// <summary>
        /// company name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        /// <summary>
        /// amount of money
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// currency
        /// </summary>
        [ForeignKey(nameof(Currency))]
        public int CurrencyId { get; }
        public Currency Currency { get; set; }

        /// <summary>
        /// date of transaction
        /// </summary>
        [Required]
        public DateTime PaymentDate { get; set; }


        public UtilityPayment(Guid id, UtilityType type, string iban, string subscriptionNo, string invoiceNo,
                              string companyName, decimal amount, int currencyId, DateTime paymentDate,
                              string createdBy, DateTime createdOn, string modifiedBy, DateTime lastModifiedOn)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            SubscriptionNo = Throw.ArgumentNullException.IfNull(subscriptionNo, nameof(subscriptionNo));
            InvoiceNo = Throw.ArgumentNullException.IfNull(invoiceNo, nameof(invoiceNo));
            CompanyName = Throw.ArgumentNullException.IfNull(companyName, nameof(companyName));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            CurrencyId = Throw.ArgumentNullException.IfNull(currencyId, nameof(currencyId));
            PaymentDate = Throw.ArgumentNullException.IfNull(paymentDate, nameof(paymentDate));
        }


        public Core.Domain.Models.UtilityPayment ToDomainModel() =>
          new Core.Domain.Models.UtilityPayment(Id,
                                                Type,
                                                IBAN,
                                                SubscriptionNo,
                                                InvoiceNo,
                                                CompanyName,
                                                CreateMoney(Amount, Currency),
                                                PaymentDate,
                                                CreateCreationInfo(CreatedBy, CreatedOn),
                                                CreateModificationInfo(LastModifiedBy, LastModifiedOn));

        private Money CreateMoney(decimal amount, Currency currency)
        {
            if (currency != null)
            {
                return new Money(new Amount(amount), new Core.Domain.Models.MoneyCurrency(currency.Id, currency.Code, currency.Symbol));
            }

            return null;
        }

        private CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) =>
           new CreationInfo(createdBy, createdOn);

        private ModificationInfo CreateModificationInfo(string lastModifiedBy, DateTime lastModifiedOn) =>
           new ModificationInfo(lastModifiedBy, lastModifiedOn);
    }
}
