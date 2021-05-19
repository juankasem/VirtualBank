using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    public class CreditCard : BaseClass
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

        [ForeignKey(nameof(BankAccount))]
        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
