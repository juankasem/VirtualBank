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

        public Amount Amount { get; set; }

        public string Currency { get; set; }

        public RecipientDetails(int bankAccountId, string iban, string bankName,
                                string recipientFullName, string recipientShortName,
                                Amount amount, string currency)
        {
            BankAccountId = bankAccountId;
            IBAN = Throw.ArgumentNullException.IfNull(IBAN, nameof(IBAN));
            BankName = Throw.ArgumentNullException.IfNull(bankName, nameof(bankName));
            RecipientFullName = Throw.ArgumentNullException.IfNull(recipientFullName, nameof(recipientFullName));
            RecipientShortName = Throw.ArgumentNullException.IfNull(recipientShortName, nameof(recipientShortName));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            Currency = Throw.ArgumentNullException.IfNull(currency, nameof(currency));
        }
    }
}