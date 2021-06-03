using System;
using System.ComponentModel.DataAnnotations;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.AuthApiRequests
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
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
