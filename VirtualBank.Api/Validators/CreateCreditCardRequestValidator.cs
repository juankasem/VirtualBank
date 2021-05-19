using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.CreditCardApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateCreditCardRequestValidator : AbstractValidator<CreateCreditCardRequest>
    {
        public CreateCreditCardRequestValidator()
        {
            RuleFor(x => x.CreditCardNo)
              .NotNull()
              .NotEmpty()
              .WithMessage("Credit card No is required")
              .MaximumLength(50)
              .Matches("^[a-zA-Z0-9_]*$");



            RuleFor(x => x.PIN)
            .NotNull()
            .NotEmpty()
            .WithMessage("PIN is required")
            .MaximumLength(4)
            .WithMessage("Max pin length is 4 characters")
            .MinimumLength(4)
            .WithMessage("Min pin length is 4 characters")
            .Matches("^[a-zA-Z0-9_]*$");



            RuleFor(x => x.ExpirationDate)
                  .NotNull()
                  .NotEmpty()
                  .WithMessage("Expiration date is required")
                  .Must(date => IsValidDate(date.ToString()))
                  .WithMessage("Invalid date format");



            RuleFor(x => x.BankAccountId)
            .NotNull()
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("PIN is required");
        }


        private bool IsValidDate(string value)
        {
            DateTime date;
            return DateTime.TryParse(value, out date);
        }
    }
}
