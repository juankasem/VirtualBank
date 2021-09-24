using System;
using VirtualBank.Core.Models;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IBankAccountMapper
    {
        BankAccount MapToBankAccount(VirtualBank.Core.Entities.BankAccount bankAccount, string accountOwner, DateTime? lastTransactionDate = null);
        RecipientBankAccount MapToRecipientBankAccount(VirtualBank.Core.Entities.BankAccount bankAccount, string accountOwner);

    }

    public class BankAccountMapper : IBankAccountMapper
    {
        public BankAccount MapToResponseModel(Core.Entities.BankAccount bankAccount)
        {
            throw new System.NotImplementedException();
        }

        public BankAccount MapToBankAccount(Core.Entities.BankAccount bankAccount, string accountOwner, DateTime? lastTransactionDate = null)
        {
            if (bankAccount != null)
            {
                return new(bankAccount.Id,
                            bankAccount.AccountNo,
                            bankAccount.IBAN,
                            bankAccount.Type,
                            accountOwner,
                            CreateBranch(bankAccount.Branch.Id, bankAccount.Branch.Code, bankAccount.Branch.Name, MapToAddress(bankAccount.Branch.Address),
                                        CreateCreationInfo(bankAccount.Branch.CreatedBy, bankAccount.Branch.CreatedOn),
                                        CreateModificationInfo(bankAccount.Branch.LastModifiedBy, (DateTime)bankAccount.Branch.LastModifiedOn)),
                            bankAccount.Balance,
                            bankAccount.AllowedBalanceToUse,
                            bankAccount.Debt,
                            bankAccount.Currency,
                            CreateCreationInfo(bankAccount.CreatedBy, bankAccount.CreatedOn),
                            CreateModificationInfo(bankAccount.LastModifiedBy, (DateTime)bankAccount.LastModifiedOn),
                            lastTransactionDate
                );
            }

            return null;
        }

        public RecipientBankAccount MapToRecipientBankAccount(Core.Entities.BankAccount bankAccount, string accountOwner)
        {
            if (bankAccount != null)
            {
                return new(bankAccount.AccountNo,
                            bankAccount.IBAN,
                            accountOwner,
                            bankAccount.Type,
                            bankAccount.Branch.Name,
                            bankAccount.Branch.Address.City.Name,
                            bankAccount.Currency.Name);
            }

            return null;
        }

        private static Address MapToAddress(Core.Entities.Address address)
        {
            throw new NotImplementedException();
        }

        private static Branch CreateBranch(int branchId, string code, string name, Address address,
                                            CreationInfo creationInfo, ModificationInfo modificationInfo) =>
        new(branchId, code, name, address, creationInfo, modificationInfo);

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string modifiededBy, DateTime lastModifiedeOn) => new(modifiededBy, lastModifiedeOn);
    }
}
