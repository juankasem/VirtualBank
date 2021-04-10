using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    public class FastTransaction : BaseClass
    {
        [ForeignKey(nameof(Account))]
        [Required]
        public int AccountId { get; set; }
        public BankAccount Account { get; set; }


        [ForeignKey(nameof(Branch))]
        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }


        [Required]
        [MaxLength(50)]
        public string RecipientName { get; set; }


        [Required]
        [MaxLength(150)]
        public string IBAN { get; set; }
    }
}
