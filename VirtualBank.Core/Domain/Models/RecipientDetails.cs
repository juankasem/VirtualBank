using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.Domain.Models
{
    public class RecipientDetails
    {
        public int BankAccountId { get; set; }

        public string IBAN { get; set; }

        public string BankName { get; set; }

        public string RecipientFullName { get; set; }

        public string RecipientShortName { get; set; }

        public Money AmountToTransfer { get; set; }


        public RecipientDetails(int bankAccountId, string iban, string bankName,
                                string recipientFullName, string recipientShortName,
                                Money amountToTransfer)
        {
            BankAccountId = bankAccountId;
            IBAN = Throw.ArgumentNullException.IfNull(IBAN, nameof(IBAN));
            BankName = Throw.ArgumentNullException.IfNull(bankName, nameof(bankName));
            RecipientFullName = Throw.ArgumentNullException.IfNull(recipientFullName, nameof(recipientFullName));
            RecipientShortName = Throw.ArgumentNullException.IfNull(recipientShortName, nameof(recipientShortName));
            AmountToTransfer = Throw.ArgumentNullException.IfNull(amountToTransfer, nameof(amountToTransfer));
        }
    }
}