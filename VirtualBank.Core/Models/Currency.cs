using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Models
{
    public class Currency : BaseData
    {
        [Required]
        [MaxLength(3)]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string Symbol { get; set; }
    }
}
