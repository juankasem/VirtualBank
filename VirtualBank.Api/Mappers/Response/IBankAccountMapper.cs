using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IBankAccountMapper
    {
        BankAccount MapToResponseModel(Core.Domain.Models.BankAccount bankAccount, DateTime? lastTransactionDate = null);
        RecipientBankAccount MapToRecipientBankAccount(Core.Domain.Models.BankAccount bankAccount);
    }

    public class BankAccountMapper : IBankAccountMapper
    {
        private readonly IBranchMapper _branchMapper;

        public BankAccountMapper(IBranchMapper branchMapper)
        {
            _branchMapper = branchMapper;
        }

        public BankAccount MapToResponseModel(Core.Domain.Models.BankAccount bankAccount, DateTime? lastTransactionDate = null) =>
            new(bankAccount.Id,
                bankAccount.AccountNo,
                bankAccount.IBAN,
                bankAccount.Type,
                bankAccount.Owner,
                new Core.Domain.Models.BankAccountBranch(bankAccount.Id, bankAccount.Branch.Code, bankAccount.Branch.Code, bankAccount.Branch.City),
                new Amount(bankAccount.Balance),
                new Amount(bankAccount.AllowedBalanceToUse),
                new Amount(bankAccount.Debt),
                new Core.Domain.Models.BankAccountCurrency(bankAccount.Currency.Id, bankAccount.Currency.Code, bankAccount.Currency.Symbol),
                bankAccount.CreationInfo,
                bankAccount.ModificationInfo,
                lastTransactionDate ?? null
                );

        public RecipientBankAccount MapToRecipientBankAccount(Core.Domain.Models.BankAccount bankAccount) =>
            new(bankAccount.AccountNo,
                bankAccount.IBAN,
                bankAccount.Owner.FullName,
                bankAccount.Type,
                bankAccount.Branch.Name,
                bankAccount.Branch.City,
                bankAccount.Currency.Code,
                bankAccount.CreationInfo,
                bankAccount.ModificationInfo);

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string modifiededBy, DateTime lastModifiedeOn) => new(modifiededBy, lastModifiedeOn);
    }
}
