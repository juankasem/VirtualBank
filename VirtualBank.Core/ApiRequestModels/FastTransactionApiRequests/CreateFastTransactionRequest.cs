using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests
{
    public class CreateFastTransactionRequest
    {
        [ForeignKey(nameof(Account))]
        [Required]
        public int AccountId { get; set; }
        public BankAccount Account { get; set; }

        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Required]
        [MaxLength(50)]
        public string RecipientName { get; set; }

        [Required]
        [MaxLength(150)]
        public string RecipientIBAN { get; set; }

        public CreateFastTransactionRequest(int accountId, int branchId,
                                            string recipientName, string recipientIBAN)
        {
            AccountId = Throw.ArgumentNullException.IfNull(accountId, nameof(accountId));
            BranchId = Throw.ArgumentNullException.IfNull(branchId, nameof(branchId));
            RecipientName = Throw.ArgumentNullException.IfNull(recipientName, nameof(recipientName));
            RecipientIBAN = Throw.ArgumentNullException.IfNull(recipientIBAN, nameof(recipientIBAN));
        }
    }
}
