using System;
using VirtualBank.Core.Models;

namespace VirtualBank.Api.Helpers.Methods
{
    public class Utils
    {
        public static string GetFullName(string firstName, string lastName) => firstName + " " + lastName;

        public static Money CreateMoney(decimal amount, string currency) => new Money(new Amount(amount), currency);

        public static CreationInfo CreateCreationInfo(string createdBy, DateTime createdOn) => new(createdBy, createdOn);

        public static ModificationInfo CreateModificationInfo(string modifiedBy, DateTime lastModifiedeOn) => new(modifiedBy, lastModifiedeOn);
    }
}