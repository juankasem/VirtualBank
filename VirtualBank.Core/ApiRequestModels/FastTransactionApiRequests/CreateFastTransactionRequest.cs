using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests
{
    public class CreateFastTransactionRequest
    {
        public int BankAccountId { get; set; }

        public int BranchId { get; set; }

        public string RecipientName { get; set; }

        public string RecipientIBAN { get; set; }

        public CreateFastTransactionRequest(int bankAccountId, int branchId,
                                            string recipientName, string recipientIBAN)
        {
            BankAccountId = Throw.ArgumentNullException.IfNull(bankAccountId, nameof(bankAccountId));
            BranchId = Throw.ArgumentNullException.IfNull(branchId, nameof(branchId));
            RecipientName = Throw.ArgumentNullException.IfNull(recipientName, nameof(recipientName));
            RecipientIBAN = Throw.ArgumentNullException.IfNull(recipientIBAN, nameof(recipientIBAN));
        }
    }
}
