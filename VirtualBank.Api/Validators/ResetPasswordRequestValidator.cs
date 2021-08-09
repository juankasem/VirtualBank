using FluentValidation;
using VirtualBank.Core.ApiRequestModels.AuthApiRequests;

namespace VirtualBank.Api.Validators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Token)
                 .NotNull()
                 .NotEmpty()
                 .WithMessage("token is required");


            RuleFor(x => x.Email)
              .NotNull()
              .NotEmpty()
              .WithMessage("email is required")
              .EmailAddress()
              .WithMessage("invalid email format");


            RuleFor(x => x.NewPassword)
              .NotNull()
              .NotEmpty()
              .WithMessage("new password is required")
              .Length(6, 100)
              .WithMessage("Password should be at least 6 characters");


            RuleFor(x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty()
            .WithMessage("Confirm Password is required")
            .Equal(x => x.NewPassword)
            .WithMessage("passwords must match");

        }
    }
}
