using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests
{
    public class CreateFastTransactionRequest
    {
        public int BankAccountId { get; set; }

        public int BranchId { get; set; }

        public string RecipientName { get; set; }

        public string RecipientIBAN { get; set; }

        public CreationInfo CreationInfo { get; set; }

        public ModificationInfo ModificationInfo { get; set; }


        public CreateFastTransactionRequest(int bankAccountId, int branchId, string recipientName, string recipientIBAN,
                                            CreationInfo creationInfo, ModificationInfo modificationInfo)
        {
            BankAccountId = Throw.ArgumentNullException.IfNull(bankAccountId, nameof(bankAccountId));
            BranchId = Throw.ArgumentNullException.IfNull(branchId, nameof(branchId));
            RecipientName = Throw.ArgumentNullException.IfNull(recipientName, nameof(recipientName));
            RecipientIBAN = Throw.ArgumentNullException.IfNull(recipientIBAN, nameof(recipientIBAN));
            CreationInfo = Throw.ArgumentNullException.IfNull(creationInfo, nameof(creationInfo));
            ModificationInfo = Throw.ArgumentNullException.IfNull(modificationInfo, nameof(modificationInfo));
        }
    }
}
