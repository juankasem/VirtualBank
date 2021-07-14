using System;

namespace VirtualBank.Core.ApiResponseModels.AuthApiResponses
{
    /// <summary>
    /// login api response model
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// user id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// displayed user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// user email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// user phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// access token for authentication
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// one time use token to renew access token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
