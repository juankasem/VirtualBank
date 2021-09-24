using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    public class FastTransaction : BaseClass
    {
        [ForeignKey(nameof(BankAccount))]
        [Required]
        public int BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }

        [Required]
        [MaxLength(50)]
        public string RecipientName { get; set; }

        [Required]
        [MaxLength(150)]
        public string RecipientIBAN { get; set; }
    }
}
