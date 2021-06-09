using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiResponseModels.LoanApiResponses
{
    public class LoanResponse
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public string IBAN { get; set; }

        public LoanType LoanType { get; set; }

        public decimal Amount { get; set; }

        public decimal InterestRate { get; set; }

        public DateTime DueDate { get; set; }


        public LoanResponse(int id, string customerName, string iban, LoanType loanType,
                            decimal amount, decimal interestRate, DateTime dueDate)
        {
            Id = Throw.ArgumentNullException.IfNull(id, nameof(id));
            CustomerName = Throw.ArgumentNullException.IfNull(customerName, nameof(customerName));
            IBAN = Throw.ArgumentNullException.IfNull(iban, nameof(iban));
            LoanType = Throw.ArgumentNullException.IfNull(loanType, nameof(loanType));
            Amount = Throw.ArgumentNullException.IfNull(amount, nameof(amount));
            InterestRate = Throw.ArgumentNullException.IfNull(interestRate, nameof(interestRate));
            DueDate = Throw.ArgumentNullException.IfNull(dueDate, nameof(dueDate));
        }
    }
}
