using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.BankAccountApiResponses
{
    public class BankAccountResponse
    {
        public BankAccount BankAccount { get; }

        public BankAccountResponse(BankAccount bankAccount)
        {
            BankAccount = Throw.ArgumentNullException.IfNull(bankAccount, nameof(bankAccount));
        }
    }
}
