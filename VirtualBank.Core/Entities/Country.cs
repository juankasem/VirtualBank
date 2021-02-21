﻿using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.Entities
{
    public class Country : BaseClass
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(8)]
        public string Code { get; set; }
    }
}