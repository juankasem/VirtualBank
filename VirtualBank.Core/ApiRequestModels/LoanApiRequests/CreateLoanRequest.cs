using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiRequestModels.LoanApiRequests
{
    public class CreateLoanRequest
    {
        public int CustomerId { get; set; }

        public int BankAccountId { get; set; }

        public LoanType LoanType { get; set; }

        public decimal Amount { get; set; }

        public decimal InterestRate { get; set; }

        public DateTime DueDate { get; set; }


        public CreateLoanRequest(int customerId, int bankAccountId, LoanType loanType, decimal amount,
                                 decimal interestRate, DateTime dueDate)
        {
            CustomerId = Throw.ArgumentNullException.IfNull(customerId, nameof(customerId));
            BankAccountId = Throw.ArgumentNullException.IfNull(bankAccountId, nameof(bankAccountId));
            LoanType = Throw.ArgumentNullException.IfNull(loanType, nameof(loanType));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            InterestRate = Throw.ArgumentNullException.IfNull(interestRate, nameof(interestRate));
            DueDate = Throw.ArgumentNullException.IfNull(dueDate, nameof(dueDate));
        }
    }
}
