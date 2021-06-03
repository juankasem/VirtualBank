using System;

namespace VirtualBank.Core.ApiResponseModels.AuthApiResponses
{
    /// <summary>
    /// login api response model
    /// </summary>
    public class LoginResponse
    {
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
