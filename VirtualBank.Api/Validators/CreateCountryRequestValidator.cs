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
                 .MaximumLength(150)
                 .Matches("^[a-zA-Z0-9_]*$");


            RuleFor(x => x.Code)
                    .NotNull()
                    .MaximumLength(4);
        }
    }
}
