using System;
using VirtualBank.Core.Enums;
using VirtualBank.Core.Models;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Api.Mappers.Response
{
    public interface ICashTransactionsMapper
    {
        CashTransaction MapToResponseModel(Core.Entities.CashTransaction cashTransaction, string iban, string sender, string recipient, Money fees = null);

        LatestTransfer MapToLatestTransferResponseModel(string toAccount, string recipient, Amount amount, DateTime createdOn);
    }

    public class CashTransactionsMapper : ICashTransactionsMapper
    {
        public CashTransaction MapToResponseModel(Core.Entities.CashTransaction cashTransaction, string iban, string sender, string recipient, Money fees = null) =>
            new(cashTransaction.Id,
                cashTransaction.From,
                cashTransaction.To,
                sender,
                recipient,
                cashTransaction.From != iban
                    ? CreateDebitedFunds(cashTransaction.Amount, cashTransaction.Currency)
                    : CreateDebitedFunds(-cashTransaction.Amount, cashTransaction.Currency),
                fees ?? CreateMoney(new Amount(0), string.Empty),
                cashTransaction.PaymentType,
                cashTransaction.From != iban ? $"From: {sender}, Account No: {cashTransaction.From} "
                : cashTransaction.InitiatedBy == BankAssetType.POS ? cashTransaction.DebitCardNo != null
                    ? $"{cashTransaction.InitiatedBy} purchase: card No: {cashTransaction.DebitCardNo}, {recipient}"
                    : $"{cashTransaction.InitiatedBy} purchase: card No: {cashTransaction.CreditCardNo}, {recipient}"
                : $"{cashTransaction.To}--{recipient}, {cashTransaction.Description}",
                cashTransaction.InitiatedBy,
                cashTransaction.From != iban
                    ? CreateMoney(new Amount(cashTransaction.RecipientRemainingBalance), cashTransaction.Currency)
                    : CreateMoney(new Amount(cashTransaction.SenderRemainingBalance), cashTransaction.Currency),
                CreateCreationInfo(cashTransaction.CreatedBy, cashTransaction.CreatedOn));


        public LatestTransfer MapToLatestTransferResponseModel(string toAccount, string recipient,
                                                               Amount amount, DateTime createdOn) =>
            new(toAccount, recipient, amount, createdOn);

        private static Money CreateDebitedFunds(decimal amount, string currency) => new(new Amount(amount), currency);

        private static Money CreateMoney(Amount amount, string currency) => new(amount, currency);

        private static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);
    }
}