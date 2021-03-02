using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Entities
{
    public class Branch : BaseClass
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Code { get; set; }

        [Required]
        public Address Address { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; }
    }
}
