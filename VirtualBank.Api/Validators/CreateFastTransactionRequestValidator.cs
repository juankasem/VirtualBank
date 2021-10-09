using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateFastTransactionRequestValidator : AbstractValidator<CreateFastTransactionRequest>
    {
        public CreateFastTransactionRequestValidator()
        {
            RuleFor(x => x.IBAN)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage("IBAN is required");


            RuleFor(x => x.RecipientIBAN)
                  .NotNull()
                  .NotEmpty()
                  .WithMessage("Recipient iban is required");


            RuleFor(x => x.RecipientFullName)
                  .NotNull()
                  .NotEmpty()
                  .WithMessage("Recipient's full name is required");


            RuleFor(x => x.Amount.Value)
                  .NotNull()
                  .NotEmpty()
                  .GreaterThan(0)
                  .WithMessage("Amount is required");


            RuleFor(x => x.CreationInfo.CreatedBy)
                  .NotNull()
                  .NotEmpty()
                  .WithMessage("created by name is required");


            RuleFor(x => x.CreationInfo.CreatedOn)
                  .NotNull()
                  .NotEmpty()
                  .WithMessage("fast transaction creation date is required")
                  .Must(date => IsValidDate(date.ToString()))
                  .WithMessage("Invalid date format");
        }


        private bool IsValidDate(string value)
        {
            DateTime date;
            return DateTime.TryParse(value, out date);
        }
    }
}
