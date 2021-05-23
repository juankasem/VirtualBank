using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.DebitCardApiResponses
{
    public class DebitCardResponse
    {
        public int Id { get; }

        public string DebitCardNo { get; }

        public DateTime ExpirationDate { get; }

        public string BankAccountNo { get; }


        public DebitCardResponse(int id, string debitCardNo, DateTime expirationDate, string bankAccountNo)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            DebitCardNo = Throw.ArgumentNullException.IfNull(debitCardNo, nameof(debitCardNo));
            ExpirationDate = Throw.ArgumentNullException.IfNull(expirationDate, nameof(expirationDate));
            BankAccountNo = Throw.ArgumentNullException.IfNull(bankAccountNo, nameof(bankAccountNo));
        }

      
    }
}
