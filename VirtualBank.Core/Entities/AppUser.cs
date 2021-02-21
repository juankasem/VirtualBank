using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        public UserType Type { get; set; }

        /// <summary>
        /// current refresh token of the app user
        /// </summary>
        [MaxLength(50)]
        public string  RefreshToken { get; set; }
    }
}
