using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IBankAccountMapper
    {
        BankAccount MapToResponseModel(VirtualBank.Core.Entities.BankAccount bankAccount, string accountOwner, DateTime? lastTransactionDate = null);
        RecipientBankAccount MapToRecipientBankAccount(VirtualBank.Core.Entities.BankAccount bankAccount, string accountOwner);

    }

    public class BankAccountMapper : IBankAccountMapper
    {
        private readonly IBranchMapper _branchMapper;

        public BankAccountMapper(IBranchMapper branchMapper)
        {
            _branchMapper = branchMapper;
        }

        public BankAccount MapToResponseModel(Core.Entities.BankAccount bankAccount, string accountOwner, DateTime? lastTransactionDate = null) =>
            new(bankAccount.Id,
                bankAccount.AccountNo,
                bankAccount.IBAN,
                bankAccount.Type,
                accountOwner,
                _branchMapper.MapToResponseModel(bankAccount.Branch),
                bankAccount.Balance,
                bankAccount.AllowedBalanceToUse,
                bankAccount.Debt,
                bankAccount.Currency,
                CreateCreationInfo(bankAccount.CreatedBy, bankAccount.CreatedOn),
                CreateModificationInfo(bankAccount.LastModifiedBy, (DateTime)bankAccount.LastModifiedOn),
                lastTransactionDate
                );

        public RecipientBankAccount MapToRecipientBankAccount(Core.Entities.BankAccount bankAccount, string accountOwner) =>
            new(bankAccount.AccountNo,
                            bankAccount.IBAN,
                            accountOwner,
                            bankAccount.Type,
                            bankAccount.Branch.Name,
                            bankAccount.Branch.Address.City.Name,
                            bankAccount.Currency.Name);

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string modifiededBy, DateTime lastModifiedeOn) => new(modifiededBy, lastModifiedeOn);
    }
}
