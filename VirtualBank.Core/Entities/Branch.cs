using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Entities
{
    public class Branch : BaseClass
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public Address Address { get; set; }

        public ICollection<Account> Accounts { get; set; }
    }
}
