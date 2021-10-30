using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiRequestModels.LoanApiRequests
{
    public class CreateLoanRequest
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string IBAN { get; set; }

        public LoanType LoanType { get; set; }

        public Money Amount { get; set; }

        public Amount InterestRate { get; set; }

        public DateTime DueDate { get; set; }

        public CreationInfo CreationInfo { get; set; }


        public CreateLoanRequest(int customerId, string customerName, string iban, LoanType loanType,
                                 Money amount, Amount interestRate, DateTime dueDate)
        {
            CustomerId = Throw.ArgumentNullException.IfNull(customerId, nameof(customerId));
            CustomerName = Throw.ArgumentNullException.IfNull(customerName, nameof(customerName));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            LoanType = Throw.ArgumentNullException.IfNull(loanType, nameof(loanType));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            InterestRate = Throw.ArgumentNullException.IfNull(interestRate, nameof(interestRate));
            DueDate = Throw.ArgumentNullException.IfNull(dueDate, nameof(dueDate));
        }
    }
}
