using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualBank.Core.Models
{
    [NotMapped]
    public class JwtOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Key { get; set; }

        public int Lifetime { get; set; }
    }
}
