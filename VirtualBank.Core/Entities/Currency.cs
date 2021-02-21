using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Entities
{
    public class Currency : BaseClass
    {
        [Required]
        [MaxLength(3)]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string Symbol { get; set; }
    }
}
