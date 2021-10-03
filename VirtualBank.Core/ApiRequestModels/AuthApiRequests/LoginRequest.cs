using VirtualBank.Core.ArgumentChecks;

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


        public LoginRequest(string customerNo, string password)
        {
            CustomerNo = Throw.ArgumentNullException.IfNull(customerNo, nameof(customerNo));
            Password = Throw.ArgumentNullException.IfNull(password, nameof(password));
        }
    }
}
