using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.LoanApiResponses
{
    public class LoanResponse
    {
        public Loan Loan { get; }


        public LoanResponse(Loan loan)
        {
            Loan = Throw.ArgumentNullException.IfNull(loan, nameof(loan));
        }
    }
}
