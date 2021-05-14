using System;
using FluentValidation;
using VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests;

namespace VirtualBank.Api.Validators
{
    public class CreateCashTransactionRequestValidator : AbstractValidator<CreateCashTransactionRequest>
    {
        public CreateCashTransactionRequestValidator()
        {
        }
    }
}
