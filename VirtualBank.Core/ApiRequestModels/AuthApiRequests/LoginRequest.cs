using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.ApiRequestModels.AuthApiRequests
{
    /// <summary>
    /// login api request model
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// customer number of the user
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// account password
        /// </summary>
        public string Password { get; set; }
    }
}
