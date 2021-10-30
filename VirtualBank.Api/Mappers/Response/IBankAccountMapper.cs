using System;
using VirtualBank.Api.Helpers.Methods;
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
                new Amount(bankAccount.Balance.Value),
                new Amount(bankAccount.AllowedBalanceToUse.Value),
                new Amount(bankAccount.Debt.Value),
                new Core.Domain.Models.BankAccountCurrency(bankAccount.Currency.Id, bankAccount.Currency.Code, bankAccount.Currency.Symbol),
                Utils.CreateCreationInfo(bankAccount.CreationInfo.CreatedBy, bankAccount.CreationInfo.CreatedOn),
                Utils.CreateModificationInfo(bankAccount.ModificationInfo.ModifiedBy, bankAccount.ModificationInfo.LastModifiedOn),
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
    }
}
