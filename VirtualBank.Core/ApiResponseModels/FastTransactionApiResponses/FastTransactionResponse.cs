using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses
{
    public class FastTransactionResponse
    {
        public int Id { get; }

        public string AccountIBAN { get; }

        public string BranchName { get; }

        public string RecipientName { get; }

        public string IBAN { get; }


        public FastTransactionResponse(int id, string accountIBAN, string branchName,
                                       string recipientName, string iban)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            AccountIBAN = Throw.ArgumentNullException.IfNull(accountIBAN, nameof(accountIBAN));
            BranchName = Throw.ArgumentNullException.IfNull(branchName, nameof(branchName));
            RecipientName = Throw.ArgumentNullException.IfNull(recipientName, nameof(recipientName));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
        }
    }
}
