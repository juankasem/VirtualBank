using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.ApiRequestModels
{

    /// <summary>
    /// signup a new user api request model
    /// </summary>
    public class SignupRequest
    {
        /// <summary>
        /// Identification Number of the customer
        /// </summary>
        [Required]
        public string IdentificationNo { get; set; }

        /// <summary>
        /// phone number
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// email
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// account password
        /// </summary>
        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string Password { get; set; }

        /// <summary>
        /// confirm account password
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "passwords must match")]
        public string ConfirmPassword { get; set; }
    }
}
