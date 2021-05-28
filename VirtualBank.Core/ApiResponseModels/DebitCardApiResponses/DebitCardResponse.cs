using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.DebitCardApiResponses
{
    public class DebitCardResponse
    {
        public int Id { get; }

        public string DebitCardNo { get; }

        public DateTime ExpirationDate { get; }

        public string IBAN{ get; }


        public DebitCardResponse(int id, string debitCardNo, DateTime expirationDate, string iban)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            DebitCardNo = Throw.ArgumentNullException.IfNull(debitCardNo, nameof(debitCardNo));
            ExpirationDate = Throw.ArgumentNullException.IfNull(expirationDate, nameof(expirationDate));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
        }  
    }
}
