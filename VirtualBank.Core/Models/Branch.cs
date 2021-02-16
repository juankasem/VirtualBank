using System;
using System.Collections.Generic;

namespace VirtualBank.Core.Models
{
    public class Branch
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public Address Address { get; set; }

        public ICollection<Account> Accounts { get; set; }
    }
}
