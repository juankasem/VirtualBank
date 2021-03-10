using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Phone]
        public string Phone { get; set; }

        [ForeignKey(nameof(Address))]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<BankAccount> BankAccounts { get; set; }
    }
}
