using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.FastTransactionApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateFastTransactionRequestValidator : AbstractValidator<CreateFastTransactionRequest>
    {
        public CreateFastTransactionRequestValidator()
        {
            RuleFor(x => x.BankAccountId)
                   .NotNull()
                   .NotEmpty()
                   .GreaterThan(0)
                   .WithMessage("Bank Account is required");


            RuleFor(x => x.BranchId)
              .NotNull()
              .NotEmpty()
              .GreaterThan(0)
              .WithMessage("Branch is required");


            RuleFor(x => x.RecipientName)
              .NotNull()
              .NotEmpty()
              .WithMessage("Recipient name is required");
              

            RuleFor(x => x.RecipientIBAN)
            .NotNull()
            .NotEmpty()
            .WithMessage("Recipient IBAN is required");
        }
    }
}
