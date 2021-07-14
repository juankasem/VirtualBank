using FluentValidation;
using VirtualBank.Core.ApiRequestModels.CityApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
    {
        public CreateCityRequestValidator()
        {
            RuleFor(x => x.Name)
                   .NotNull()
                   .NotEmpty()
                   .MaximumLength(150)
                   .Matches("^[a-zA-Z0-9_]*$");


            RuleFor(x => x.CountryId)
                    .NotNull()
                    .GreaterThan(0);
        }
    }
}
