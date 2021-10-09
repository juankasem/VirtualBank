using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Entities
{
    public class FastTransaction : BaseClass
    {
        [Required]
        [MaxLength(50)]
        public string IBAN { get; set; }

        [ForeignKey(nameof(RecipientBankAccount))]
        [Required]
        public int RecipientBankAccountId { get; set; }
        public BankAccount RecipientBankAccount { get; set; }

        [Required]
        [MaxLength(150)]
        public string RecipientFullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string RecipientShortName { get; set; }

        [Required]
        public decimal Amount { get; set; }


        public FastTransaction(int id, string iban, int recipientBankAccountId, string recipientFullName, string recipientShortName,
                               decimal amount, string createdBy, DateTime createdOn,
                               string modifiedBy, DateTime lastModifiedOn)
        {
            Id = id;
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            RecipientBankAccountId = Throw.ArgumentNullException.IfNull(recipientBankAccountId, nameof(recipientBankAccountId));
            RecipientFullName = Throw.ArgumentNullException.IfNull(recipientFullName, nameof(recipientFullName));
            RecipientShortName = Throw.ArgumentNullException.IfNull(recipientShortName, nameof(recipientShortName));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            CreatedBy = Throw.ArgumentNullException.IfNull(createdBy, nameof(createdBy));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            LastModifiedBy = Throw.ArgumentNullException.IfNull(modifiedBy, nameof(modifiedBy));
            LastModifiedOn = Throw.ArgumentNullException.IfNull(lastModifiedOn, nameof(lastModifiedOn));
        }

        public Core.Domain.Models.FastTransaction ToDomainModel() =>
          new Core.Domain.Models.FastTransaction(Id,
                                                 IBAN,
                                                 new Domain.Models.RecipientDetails(RecipientBankAccountId, RecipientBankAccount.IBAN,
                                                                                                   RecipientBankAccount.Branch.Name,
                                                                                                   RecipientFullName, RecipientShortName,
                                                                                                   CreateMoney(Amount, RecipientBankAccount.Currency.Code)),
                                                 new CreationInfo(CreatedBy, CreatedOn),
                                                 new ModificationInfo(LastModifiedBy, LastModifiedOn)
                                            );

        private Core.Models.Money CreateMoney(decimal amount, string currency) =>
             new Core.Models.Money(new Amount(amount), currency);
    }
}
