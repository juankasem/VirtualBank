using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateCashTransactionRequestValidator : AbstractValidator<CreateCashTransactionRequest>
    {
        public CreateCashTransactionRequestValidator()
        {
            RuleFor(x => x.Type)
                    .NotNull()
                    .WithMessage("cash transaction type is required");


            RuleFor(x => x.InitiatedBy)
                    .NotNull()
                    .WithMessage("initiated by is required");


            RuleFor(x => x.From)
                    .MaximumLength(50)
                    .Matches("^[a-zA-Z0-9_]*$")
                    .WithMessage("invalid format for IBAN");


            RuleFor(x => x.To)
                    .MaximumLength(50)
                    .Matches("^[a-zA-Z0-9_]*$")
                    .WithMessage("invalid format for IBAN");


            RuleFor(x => x.Amount)
                    .NotNull()
                    .NotEmpty()
                    .ScalePrecision(8, 2)
                    .WithMessage("Invalid format for amount");


            RuleFor(x => x.PaymentType)
                  .NotNull()
                  .NotEmpty()
                  .WithMessage("payment type is required");


            RuleFor(x => x.TransactionDate)
                  .NotNull()
                  .NotEmpty()
                  .WithMessage("Transaction date is required")
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
