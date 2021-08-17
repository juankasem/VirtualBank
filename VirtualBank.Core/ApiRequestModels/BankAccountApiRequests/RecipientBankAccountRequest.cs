using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.BankAccountApiRequests
{
    public class RecipientBankAccountRequest
    {
        public string RecipientName { get; set; }

        public string IBAN { get; set; }

        public RecipientBankAccountRequest(string recipientName, string iban)
        {
            RecipientName = Throw.ArgumentNullException.IfNull(recipientName, nameof(recipientName));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
        }
    }
}
