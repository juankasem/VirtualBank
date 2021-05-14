using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.AddressApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateAddressRequestValidator : AbstractValidator<CreateAddressRequest>
    {
        public CreateAddressRequestValidator()
        {
            RuleFor(x => x.Name)
                        .NotNull()
                        .NotEmpty()
                        .MaximumLength(50)
                        .Matches("^[a-zA-Z0-9_]*$");

            RuleFor(x => x.DistrictId)
                        .NotNull()
                        .NotEqual(0);

            RuleFor(x => x.CityId)
                        .NotNull()
                        .NotEqual(0);

            RuleFor(x => x.CountryId)
                        .NotNull()
                        .NotEqual(0);

            RuleFor(x => x.Street)
                        .NotNull()
                        .NotEmpty()
                        .MaximumLength(50);

            RuleFor(x => x.PostalCode)
                    .NotNull()
                    .NotEmpty()
                    .MaximumLength(50)
                    .Matches("^[a-zA-Z0-9_]*$");
        }
    }
}
