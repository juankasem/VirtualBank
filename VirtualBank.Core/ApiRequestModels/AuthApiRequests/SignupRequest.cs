using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.ApiRequestModels.AuthApiRequests
{
    public class SignupRequest
    {
        /// <summary>
        /// Identification Number of the customer
        /// </summary>
        public string IdentificationNo { get; set; }

        /// <summary>
        ///  customer number
        /// </summary>
        public string CustomerNo { get; set; }

        /// <summary>
        /// phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// account password
        /// </summary>
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// confirm account password
        /// </summary>
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
