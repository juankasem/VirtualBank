using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CreditCardApiResponses
{
    public class CreditCardResponse
    {
        public int Id { get; }

        public string CreditCardNo { get; }

        public DateTime ExpirationDate { get;  }

        public string IBAN { get; }


        public CreditCardResponse(int id, string creditCardNo, DateTime expirationDate, string iban)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            CreditCardNo = Throw.ArgumentNullException.IfNull(creditCardNo, nameof(creditCardNo));
            ExpirationDate = Throw.ArgumentNullException.IfNull(expirationDate, nameof(expirationDate));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
        }
    }
}
