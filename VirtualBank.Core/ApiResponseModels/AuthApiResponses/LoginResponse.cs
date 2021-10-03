using System.Diagnostics;
using System;
using VirtualBank.Core.ArgumentChecks;

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
        public string Id { get; }

        /// <summary>
        /// displayed user name
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// user email
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// user phone number
        /// </summary>
        public string Phone { get; }

        /// <summary>
        /// access token for authentication
        /// </summary>
        public string AccessToken { get; }

        /// <summary>
        /// one time use token to renew access token
        /// </summary>
        public string RefreshToken { get; }

        public DateTime LastSuccessfulLogin { get; } = DateTime.Now;

        public LoginResponse(string id, string username, string email, string phone, string accessToken, string refreshToken)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            Username = Throw.ArgumentNullException.IfNull(username, nameof(username));
            Email = Throw.ArgumentNullException.IfNull(email, nameof(email));
            Phone = Throw.ArgumentNullException.IfNull(phone, nameof(phone));
            AccessToken = Throw.ArgumentNullException.IfNull(accessToken, nameof(accessToken));
            RefreshToken = Throw.ArgumentNullException.IfNull(refreshToken, nameof(refreshToken));
        }
    }
}
