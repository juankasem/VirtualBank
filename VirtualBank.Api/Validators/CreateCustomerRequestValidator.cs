using FluentValidation;
using VirtualBank.Core.ApiRequestModels.CustomerApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.IdentificationNo)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Identification No is required")
                    .Matches("^[a-zA-Z0-9]*$")
                    .WithMessage("Identification No should contain only alphanumeric values");


            RuleFor(x => x.IdentificationType)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Identification Type is required");
          

            RuleFor(x => x.TaxNumber)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Tax Number is required");


            RuleFor(x => x.FirstName)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("First Name is required");


            RuleFor(x => x.LastName)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Last Name is required");


            RuleFor(x => x.FatherName)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Father Name is required")
                    .MaximumLength(150)
                    .WithMessage("Father Name should not exceed 150 characters");


            RuleFor(x => x.Nationality)
                .NotNull()
                .NotEmpty()
                .WithMessage("Nationality is required")
                .MaximumLength(150)
                .WithMessage("Nationality should not exceed 150 characters");


            RuleFor(x => x.Gender)
                .NotNull()
                .NotEmpty()
                .WithMessage("Gender is required");


            RuleFor(x => x.BirthDate)
                 .NotNull()
                 .NotEmpty()
                 .WithMessage("Birth Date is required");


            RuleFor(x => x.UserId)
                 .NotNull()
                 .NotEmpty()
                 .WithMessage("UserId is required");

            RuleFor(x => x.Address)
                .NotNull()
                .NotEmpty()
                .WithMessage("Address is required");
        }
    }
}
