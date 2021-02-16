using System;
namespace VirtualBank.Core.ApiResponseModels
{
    public class TokenResponse
    {
        /// <summary>
        /// the new access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// one time used refresh token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
