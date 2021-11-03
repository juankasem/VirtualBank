using System;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Models;

namespace VirtualBank.Api.Helpers.Methods
{
    public class Utils
    {
        public static string GetFullName(string firstName, string lastName) => firstName + " " + lastName;

        public static Money CreateMoney(decimal amount, MoneyCurrency currency) =>
                   new(new Amount(amount), new MoneyCurrency(currency.Id, currency.Code, currency.Symbol));

        public static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        public static ModificationInfo CreateModificationInfo(string modifiedBy, DateTime lastModifiedeOn) => new(modifiedBy, lastModifiedeOn);
    }
}