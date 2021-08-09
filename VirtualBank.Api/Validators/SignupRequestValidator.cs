using FluentValidation;
using VirtualBank.Api.Services;
using VirtualBank.Core.ApiRequestModels.AuthApiRequests;

namespace VirtualBank.Api.Validators
{
    public class SignupRequestValidator : AbstractValidator<SignupRequest>
    {
        private readonly AuthService _authService;

        public SignupRequestValidator(AuthService authService)
        {
            _authService = authService;

            RuleFor(x => x.IdentificationNo)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Identification No is required")
                    .Matches("^[a-zA-Z0-9]*$")
                    .WithMessage("Identification No should contain only alphanumeric values");


            RuleFor(x => x.CustomerNo)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Customer No is required");


            RuleFor(x => x.PhoneNumber)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Phone Number is required")
                    .Matches("^[a-zA-Z0-9]*$")
                    .WithMessage("Phone Number should contain match the pattern");


            RuleFor(x => x.Email)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("First Name is required")
                    .EmailAddress()
                    .WithMessage("Invalid Email format");


            RuleFor(x => x.Password)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Password is required")
                    .Length(6, 100)
                    .WithMessage("Password should be at least 6 characters");


            RuleFor(x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty()
            .WithMessage("Confirm Password is required")
            .Equal(x => x.Password)
            .WithMessage("passwords must match");
        }
    }
}
