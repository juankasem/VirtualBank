using System;
namespace VirtualBank.Core.ApiResponseModels
{
    /// <summary>
    /// login api response model
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// token for authentication
        /// </summary>
        public string Token { get; set; }
    }
}
