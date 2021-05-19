using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.AccountApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateBankAccountRequestValidator : AbstractValidator<CreateBankAccountRequest>
    {
        public CreateBankAccountRequestValidator()
        {
            RuleFor(x => x.AccountNo)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Account number is required")
                    .Matches("^[a-zA-Z0-9]*$")
                    .WithMessage("Account No should contain only alphanumeric values");


            RuleFor(x => x.IBAN)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("IBAN is required")
                    .Matches("^[a-zA-Z0-9]*$")
                    .WithMessage("IBAN should contain only alpha numeric values");


            RuleFor(x => x.Type)
                    .NotNull()
                    .WithMessage("Account type is required");


            RuleFor(x => x.CustomerId)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Customer is required");


            RuleFor(x => x.BranchId)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Branch is required");


            RuleFor(x => x.CurrencyId)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Currency is required");


            RuleFor(x => x.MinimumAllowedBalance)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("Minimum allowed balance should be equal or greater than 1");
        }
    }
}
