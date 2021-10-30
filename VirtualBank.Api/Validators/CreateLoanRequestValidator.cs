using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.LoanApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
    {
        public CreateLoanRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                 .NotNull()
                 .NotEmpty()
                 .GreaterThan(0)
                 .WithMessage("customer is required");

            RuleFor(x => x.IBAN)
              .NotNull()
              .NotEmpty()
              .WithMessage("IBAN is required");

            RuleFor(x => x.LoanType)
              .NotNull()
              .NotEmpty()
              .WithMessage("Loan Type is required");

            RuleFor(x => x.Amount)
            .NotNull()
            .NotEmpty()
            .WithMessage("Amount is required");

            RuleFor(x => x.Amount.Amount.Value)
           .GreaterThan(0)
           .WithMessage("loan amount should be gtreater than 0");

            RuleFor(x => x.InterestRate)
              .NotNull()
              .NotEmpty()
              .WithMessage("Interest Rate is required");

            RuleFor(x => x.InterestRate.Value)
              .GreaterThan(0)
              .WithMessage("Interest Rate should be gtreater than 0");

            RuleFor(x => x.DueDate)
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
