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
                bankAccount.Owner.FullName,
                new BankAccount.Branch(bankAccount.AccountBranch.Id, bankAccount.AccountBranch.Name, bankAccount.AccountBranch.Code),
                new Amount(bankAccount.Balance),
                new Amount(bankAccount.AllowedBalanceToUse),
                new Amount(bankAccount.Debt),
                new BankAccount.Currency(bankAccount.AccountCurrency.Id, bankAccount.AccountCurrency.Code, bankAccount.AccountCurrency.Symbol),
                bankAccount.CreationInfo,
                bankAccount.ModificationInfo,
                lastTransactionDate ?? null
                );

        public RecipientBankAccount MapToRecipientBankAccount(Core.Domain.Models.BankAccount bankAccount) =>
            new(bankAccount.AccountNo,
                bankAccount.IBAN,
                bankAccount.Owner.FullName,
                bankAccount.Type,
                bankAccount.AccountBranch.Name,
                bankAccount.AccountBranch.City,
                bankAccount.AccountCurrency.Code,
                bankAccount.CreationInfo,
                bankAccount.ModificationInfo);

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string modifiededBy, DateTime lastModifiedeOn) => new(modifiededBy, lastModifiedeOn);
    }
}
