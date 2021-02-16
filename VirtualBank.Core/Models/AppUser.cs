using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models
{
    public class AppUser : IdentityUser
    {

        [Required]
        public UserType Type { get; set; }

    }
}
