using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CreditCardApiResponses
{
    public class CreditCardResponse
    {
        public int Id { get; }

        public string CreditCardNo { get; }

        public DateTime ExpirationDate { get;  }

        public int AccountId { get; }


        public CreditCardResponse(int id, string creditCardNo, DateTime expirationDate, int accountId)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            CreditCardNo = Throw.ArgumentNullException.IfNull(creditCardNo, nameof(creditCardNo));
            ExpirationDate = Throw.ArgumentNullException.IfNull(expirationDate, nameof(expirationDate));
            AccountId = Throw.ArgumentNullException.IfNull(accountId, nameof(accountId));
        }
    }
}
