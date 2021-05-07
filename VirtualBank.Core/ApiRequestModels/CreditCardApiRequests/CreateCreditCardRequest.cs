using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.CreditCardApiRequests
{
    public class CreateCreditCardRequest
    {
        [Required]
        [MaxLength(50)]
        public string CreditCardNo { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(4)]
        public string PIN { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public int AccountId { get; set; }


        public CreateCreditCardRequest(string creditCardNo, string pin, DateTime expirationDate, int accountId)
        {
            CreditCardNo = Throw.ArgumentNullException.IfNull(creditCardNo, nameof(creditCardNo));
            PIN = Throw.ArgumentNullException.IfNull(pin, nameof(pin));
            ExpirationDate = Throw.ArgumentOutOfRangeException.IfLessThan(expirationDate, DateTime.Now, nameof(expirationDate));
            AccountId = Throw.ArgumentNullException.IfNull(accountId, nameof(accountId));
        }
    }
}
