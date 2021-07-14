using FluentValidation;
using VirtualBank.Core.ApiRequestModels.CreditCardApiRequests;

namespace VirtualBank.Api.Validators
{
    public class ValidateCreditCardPINRequestValidator : AbstractValidator<ValidateCreditCardPINRequest>
    {
        public ValidateCreditCardPINRequestValidator()
        {
            RuleFor(x => x.CreditCardNo)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage("Credit Card No is required");


            RuleFor(x => x.PIN)
                  .NotNull()
                  .NotEmpty()
                  .WithMessage("PIN is required");
        }
    }
}
