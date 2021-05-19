using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateCountryRequestValidator : AbstractValidator<CreateCountryRequest>
    {
        public CreateCountryRequestValidator()
        {
            RuleFor(x => x.Name)
                 .NotNull()
                 .NotEmpty()
                 .WithMessage("country name is required")
                 .MaximumLength(150)
                 .WithMessage("Max country name's length is 150 characters")
                 .Matches("^[a-zA-Z0-9_]*$");


            RuleFor(x => x.Code)
                    .NotNull()
                     .NotEmpty()
                    .WithMessage("country code is required")
                    .MaximumLength(4)
                    .WithMessage("Max country code's length is 4 characters");
        }
    }
}
