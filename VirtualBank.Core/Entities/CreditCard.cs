using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Entities
{
    public class CreditCard : BaseClass
    {
        [Required]
        public string Number { get; set; }

        [Required]
        public string PIN { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [ForeignKey("Account")]
        public string AccountId { get; set; }

        public Account Account { get; set; }

    }
}
