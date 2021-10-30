using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IFastTransactionsMapper
    {
        FastTransaction MapToResponseModel(Core.Domain.Models.FastTransaction fastTransaction);
    }

    public class FastTransactionsMapper : IFastTransactionsMapper
    {
        public FastTransaction MapToResponseModel(Core.Domain.Models.FastTransaction fastTransaction) =>
            new(fastTransaction.Id,
                fastTransaction.IBAN,
                fastTransaction.RecipientDetails,
                fastTransaction.CreationInfo,
                fastTransaction.ModificationInfo);
    }
}
