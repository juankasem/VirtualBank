using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.ApiRequestModels
{
    /// <summary>
    /// login api request model
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// customer number of the user
        /// </summary>
        [Required]
        public string CustomerNo { get; set; }

        /// <summary>
        /// account password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
