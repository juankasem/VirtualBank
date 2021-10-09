using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests
{
    public class CreateFastTransactionRequest
    {
        public string IBAN { get; set; }

        public string RecipientFullName { get; set; }

        public string RecipientShortName { get; set; }

        public string RecipientIBAN { get; set; }

        public Money AmountToTransfer { get; set; }

        public CreationInfo CreationInfo { get; set; }


        public CreateFastTransactionRequest(string iban, string recipientFullName, string recipientShortName,
                                            string recipientIBAN, Money amountToTransfer, CreationInfo creationInfo)
        {
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            RecipientFullName = Throw.ArgumentNullException.IfNull(recipientFullName, nameof(recipientFullName));
            RecipientShortName = Throw.ArgumentNullException.IfNull(recipientShortName, nameof(recipientShortName));
            RecipientIBAN = Throw.ArgumentNullException.IfNull(recipientIBAN, nameof(recipientIBAN));
            AmountToTransfer = Throw.ArgumentNullException.IfNull(amountToTransfer, nameof(amountToTransfer));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
        }
    }
}
