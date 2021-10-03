using System;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface ICashTransactionsMapper
    {
        CashTransaction MapToResponseModel(Core.Domain.Models.CashTransaction cashTransaction, string iban, string sender, string recipient, Money fees = null);

        LatestTransfer MapToLatestTransferResponseModel(Core.Domain.Models.CashTransaction cashTransaction, string recipient);
    }

    public class CashTransactionsMapper : ICashTransactionsMapper
    {
        public CashTransaction MapToResponseModel(Core.Domain.Models.CashTransaction cashTransaction,
                                                  string iban, string sender,
                                                  string recipient, Money fees = null) =>
            new(cashTransaction.Id,
                cashTransaction.ReferenceNo,
                cashTransaction.InitiatedBy,
                cashTransaction.From,
                cashTransaction.To,
                sender,
                recipient,
                cashTransaction.From != iban
                    ? CreateDebitedFunds(cashTransaction.DebitedFunds.Amount, cashTransaction.DebitedFunds.Currency)
                    : CreateDebitedFunds(new Amount(-cashTransaction.DebitedFunds.Amount), cashTransaction.DebitedFunds.Currency),
                fees ?? CreateMoney(new Amount(0), string.Empty),
                cashTransaction.PaymentType,
                cashTransaction.From != iban ? $"From: {sender}, Account No: {cashTransaction.From} "
                : cashTransaction.InitiatedBy == BankAssetType.POS ? cashTransaction.DebitCardNo != null
                    ? $"{cashTransaction.InitiatedBy} purchase: card No: {cashTransaction.DebitCardNo}, {recipient}"
                    : $"{cashTransaction.InitiatedBy} purchase: card No: {cashTransaction.CreditCardNo}, {recipient}"
                : $"{cashTransaction.To}--{recipient}, {cashTransaction.Description}",
                cashTransaction.From != iban
                    ? cashTransaction.RecipientRemainingBalance
                    : cashTransaction.SenderRemainingBalance,
                cashTransaction.TransactionDate,
                cashTransaction.CreationInfo,
                cashTransaction.ModificationInfo,
                cashTransaction.CreditCardNo != null ? cashTransaction.CreditCardNo : null,
                cashTransaction.DebitCardNo != null ? cashTransaction.DebitCardNo : null
                );


        public LatestTransfer MapToLatestTransferResponseModel(Core.Domain.Models.CashTransaction cashTransaction, string recipient) =>
            new(cashTransaction.To,
                recipient,
                new Amount(cashTransaction.DebitedFunds.Amount),
                cashTransaction.TransactionDate,
                cashTransaction.CreationInfo,
                cashTransaction.ModificationInfo);

        private static Money CreateDebitedFunds(Amount amount, string currency) => new(amount, currency);

        private static Money CreateMoney(Amount amount, string currency) => new(amount, currency);

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        private static ModificationInfo CreateModificationInfo(string modifiedBy, DateTime modifiedOn) => new(modifiedBy, modifiedOn);

    }
}