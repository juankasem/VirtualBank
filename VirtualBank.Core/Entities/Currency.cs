using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Entities
{
    public class Currency : BaseClass
    {
        [Required]
        [MaxLength(6)]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(4)]
        public string Symbol { get; set; }
    }
}
