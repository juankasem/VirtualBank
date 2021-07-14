using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.CreditCardApiResponses
{
    public class CreditCardResponse
    {
        public int Id { get; }

        public string CreditCardNo { get; }

        public string CreditCardHolder { get; }

        public string IBAN { get; }

        public DateTime ExpirationDate { get;  }

        public DateTime CreatedOn { get; }

        public DateTime LastModifiedOn { get; }


        public CreditCardResponse(int id, string creditCardNo, string creditCardHolder, string iban,
                                  DateTime expirationDate, DateTime createdOn, DateTime lastModifiedOn)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            CreditCardNo = Throw.ArgumentNullException.IfNull(creditCardNo, nameof(creditCardNo));
            CreditCardHolder = Throw.ArgumentNullException.IfNull(creditCardHolder, nameof(creditCardHolder));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            ExpirationDate = Throw.ArgumentNullException.IfNull(expirationDate, nameof(expirationDate));
            CreatedOn = Throw.ArgumentNullException.IfNull(createdOn, nameof(createdOn));
            LastModifiedOn = Throw.ArgumentNullException.IfNull(lastModifiedOn, nameof(lastModifiedOn));
        }
    }
}
