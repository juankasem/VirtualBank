using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiRequestModels.RecipientApiRequests
{
    public class CreateRecipientRequest
    {
        [Required]
        [MaxLength(150)]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(50)]
        public string IBAN { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }

        [MaxLength(50)]
        public string ShortName { get; set; }


        public CreateRecipientRequest(int accountId, string iban, string fullName, string shortName)
        {
            AccountId = Throw.ArgumentNullException.IfNull(accountId, nameof(accountId));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            FullName = Throw.ArgumentNullException.IfNull(fullName, nameof(fullName));
            ShortName = shortName;
        }
    }
}
