using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IFastTransactionsMapper
    {
        FastTransaction MapToResponseModel(VirtualBank.Core.Entities.FastTransaction fastTransaction);
    }

    public class FastTransactionsMapper : IFastTransactionsMapper
    {
        public FastTransaction MapToResponseModel(Core.Entities.FastTransaction fastTransaction) =>
            new(fastTransaction.Id,
                fastTransaction.BankAccount.IBAN,
                fastTransaction.BankAccount.Branch.Name,
                fastTransaction.RecipientName,
                fastTransaction.RecipientIBAN,
                CreateCreationInfo(fastTransaction.CreatedBy, fastTransaction.CreatedOn),
                CreateModificationInfo(fastTransaction.LastModifiedBy, fastTransaction.LastModifiedOn));

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string lastModifiedBy, DateTime lastModifiedOn) => new(lastModifiedBy, lastModifiedOn);
    }
}
