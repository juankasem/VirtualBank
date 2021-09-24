using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.BankAccountApiResponses
{
    public class RecipientBankAccountResponse
    {
        public RecipientBankAccount RecipientBankAccount { get; }

        public RecipientBankAccountResponse(RecipientBankAccount recipientBankAccount)
        {
            RecipientBankAccount = Throw.ArgumentNullException.IfNull(recipientBankAccount, nameof(recipientBankAccount));
        }
    }
}
