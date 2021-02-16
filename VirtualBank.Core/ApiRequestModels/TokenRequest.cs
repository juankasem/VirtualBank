using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.ApiRequestModels
{
    public class TokenRequest
    {
        /// <summary>
        /// the expired access token
        /// </summary>
        [Required]
        public string AccessToken { get; set; }

        /// <summary>
        /// refresh token used to renew the access token
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }

    }
}
