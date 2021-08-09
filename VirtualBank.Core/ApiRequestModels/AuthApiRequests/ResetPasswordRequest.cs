using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.AuthApiRequests
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }

        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public ResetPasswordRequest(string token, string email,  string newPassword, string confirmPassword)
        {
            Token = Throw.ArgumentNullException.IfNull(token, nameof(token));
            Email = Throw.ArgumentNullException.IfNull(email, nameof(email));
            NewPassword = Throw.ArgumentNullException.IfNull(newPassword, nameof(newPassword));
            ConfirmPassword = Throw.ArgumentNullException.IfNull(confirmPassword, nameof(confirmPassword));
        }
    }
}
