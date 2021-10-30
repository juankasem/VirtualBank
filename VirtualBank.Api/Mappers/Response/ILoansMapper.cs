using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface ILoansMapper
    {
        Loan MapToResponseModel(Core.Domain.Models.Loan loan);
    }

    public class LoansMapper : ILoansMapper
    {
        public Loan MapToResponseModel(Core.Domain.Models.Loan loan) =>
            new(loan.Id.ToString(),
                loan.BankAccountCustomer,
                loan.LoanType,
                loan.Amount,
                loan.InterestRate,
                loan.DueDate,
                loan.CreationInfo,
                loan.ModificationInfo);
    }
}