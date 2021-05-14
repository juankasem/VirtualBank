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
                    .Matches("^[a-zA-Z0-9]*$");

            RuleFor(x => x.IBAN)
                    .NotNull()
                    .NotEmpty()
                    .Matches("^[a-zA-Z0-9]*$");

            RuleFor(x => x.Type)
                    .NotNull();

            RuleFor(x => x.CustomerId)
                    .NotEqual(0);

            RuleFor(x => x.BranchId)
                    .NotEqual(0);

            RuleFor(x => x.CurrencyId)
                    .NotEqual(0);

            RuleFor(x => x.MinimumAllowedBalance)
                    .GreaterThanOrEqualTo(1);
        }
    }
}
