using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Models
{
    public class Branch : BaseData
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
