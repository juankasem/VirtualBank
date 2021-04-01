using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    public class Recipient : BaseClass
    {
        [ForeignKey(nameof(Account))]
        [MaxLength(150)]
        public int AccountId { get; set; }
        public BankAccount Account{ get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; }

        [MaxLength(50)]
        public string ShortName { get; set; }
 
        [Required]
        [MaxLength(50)]
        public string IBAN { get; set; }
    }
}
