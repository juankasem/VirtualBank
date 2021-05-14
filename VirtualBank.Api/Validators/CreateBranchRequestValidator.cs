using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.BranchApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
    {
        public CreateBranchRequestValidator()
        {
            RuleFor(x => x.Name)
                   .NotNull()
                   .NotEmpty()
                   .Matches("^[a-zA-Z0-9_]*$");

            RuleFor(x => x.Code)
                    .NotNull()
                    .NotEmpty()
                    .Matches("^[a-zA-Z0-9]*$");

            RuleFor(x => x.Phone)
                    .NotNull()
                    .NotEmpty()
                    .Matches("^[a-zA-Z0-9_]*$");

            RuleFor(x => x.Address)
                    .NotNull();
        }
    }
}
