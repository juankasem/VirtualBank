using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.AuthApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateLoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public CreateLoginRequestValidator()
        {
            RuleFor(x => x.CustomerNo)
                .NotNull()
                .NotEmpty()
                .WithMessage("Customer No is required");


            RuleFor(x => x.Password)
              .NotNull()
              .NotEmpty()
              .WithMessage("Password is required");
        }
    }
}
