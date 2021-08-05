using System;
namespace VirtualBank.Core.ApiRoutes
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base  = Root + "/" + Version;


        public static class Addresses
        {
            public const string controller = "address";

            public const string GetAll = Base + "/" + controller + "/all";

            public const string GetById = Base + "/" + controller + "/{addressId:int}";

            public const string Post = Base + "/" + controller + "/{addressId:int}";

            public const string Delete = Base + "/" + controller + "/{addressId:int}";
        }


        public static class Administration
        {
            public const string controller = "administration";

            public const string ListRoles = Base + "/" + controller + "/roles/list";

            public const string GetUsersInRole = Base + "/" + controller + "/roles/users";

            public const string EditUsersInRole = Base + "/" + controller + "/roles/users";

            public const string CreateRole = Base + "/" + controller + "/roles";
        }


        public static class Auth
        {
            public const string controller = "auth";

            public const string Register = Base + "/" + controller + "/register";

            public const string Login = Base + "/" + controller + "/login";

            public const string ForgotPassword = Base + "/" + controller + "/forgot-password";

            public const string ConfirmEmail = Base + "/" + controller + "/confirm-email";

            public const string CheckEmailExists = Base + "/" + controller + "/check-email-exists";

            public const string ResetPassword = Base + "/" + controller + "/reset-password";
        }


        public static class BankAccounts
        {
            public const string controller = "bank-account";

            public const string GetByCustomerId = Base + "/" + controller + "/customer/{customerId:int}";

            public const string GetById = Base + "/" + controller + "/{accountId:int}";

            public const string GetByAccountNo = Base + "/" + controller + "/account-no/{accountNo}";

            public const string GetByIBAN = Base + "/" + controller + "/iban/{iban}";

            public const string ValidateRecipient = Base + "/" + controller + "/validate-recipient/";

            public const string Post = Base + "/" + controller + "/{accountId:int}";

            public const string Activate = Base + "/" + controller + "/activate/{accountId:int}";

            public const string Deactivate = Base + "/" + controller + "/deactivate/{accountId:int}";
        }


        public static class Branches
        {
            public const string controller = "branch";

            public const string List = Base + "/" + controller + "/list";

            public const string Search = Base + "/" + controller + "/search";

            public const string GetByCityId = Base + "/" + controller + "/city/{cityId:int}";

            public const string GetById = Base + "/" + controller + "/{branchId:int}";

            public const string GetByCode = Base + "/" + controller + "/code/{code}";

            public const string Post = Base + "/" + controller + "/{branchId:int}";

            public const string Delete = Base + "/" + controller + "/{branchId:int}";
        }


        public static class Districts
        {
            public const string controller = "districts";

            public const string List = Base + "/" + controller + "/list";

            public const string GetByCityId = Base + "/" + controller + "/city/{cityId:int}";

            public const string GetById = Base + "/" + controller + "/{districtId:int}";

            public const string Post = Base + "/" + controller + "/{districtId:int}";
        }


        public static class Cities
        {
            public const string controller = "cities";

            public const string List = Base + "/" + controller + "/list";

            public const string GetByCountryId = Base + "/" + controller + "/country/{countryId:int}";

            public const string GetById = Base + "/" + controller + "/{cityId:int}";

            public const string Post = Base + "/" + controller + "/{cityId:int}";
        }


        public static class Countries
        {
            public const string controller = "countries";

            public const string GetAll = Base + "/" + controller + "/all";

            public const string GetById = Base + "/" + controller + "/{countryId:int}";

            public const string Post = Base + "/" + controller + "/{countryId:int}";
        }


        public static class CashTransactions
        {
            public const string controller = "cash-transactions";

            public const string GetAll = Base + "/" + controller + "/all";

            public const string GetByAccountNo = Base + "/" + controller + "/list/account-no/{accountNo}";

            public const string GetByIBAN = Base + "/" + controller + "/list/iban/{iban}";

            public const string GetLatestByIBAN = Base + "/" + controller + "/latest-transactions/{iban}";

            public const string Post = Base + "/" + controller;
        }


        public static class CreditCards
        {
            public const string controller = "credit-cards";

            public const string GetAll = Base + "/" + controller + "/all";

            public const string GetByIBAN = Base + "/" + controller + "/iban/{iban}";

            public const string GetById = Base + "/" + controller + "/{creditCardId:int}";

            public const string GetByCreditCardNo = Base + "/" + controller + "/card-no/{creditCardNo";

            public const string GetByAccountNo = Base + "/" + controller + "/account/{accountNo}";

            public const string Post = Base + "/" + controller + "/{customerId:int}";

            public const string Activate = Base + "/" + controller + "/activate/{creditCardId:int}";

            public const string Deactivate = Base + "/" + controller + "/deactivate/{creditCardId:int}";
        }


        public static class Customers
        {
            public const string controller = "customers";

            public const string GetAll = Base + "/" + controller + "/all";

            public const string Search = Base + "/" + controller + "/search";

            public const string GetById = Base + "/" + controller + "/{customerId:int}";

            public const string GetByAccountNo = Base + "/" + controller + "/account/{accountNo}";

            public const string GetByIBAN = Base + "/" + controller + "/iban/{iban}";

            public const string Post = Base + "/" + controller + "/{customerId:int}";
        }


        public static class DebitCards
        {
            public const string controller = "debit-cards";

            public const string GetAll = Base + "/" + controller + "/all";

            public const string GetById = Base + "/" + controller + "/{debitCardId:int}";

            public const string GetByDebitCardNo = Base + "/" + controller + "/card-no/{debitCardNo";

            public const string GetByAccountNo = Base + "/" + controller + "/account/{accountNo}";

            public const string Post = Base + "/" + controller + "/{customerId:int}";

            public const string Activate = Base + "/" + controller + "/activate/{debitCardId:int}";

            public const string Deactivate = Base + "/" + controller + "/deactivate/{debitCardId:int}";
        }


        public static class FastTransactions
        {
            public const string controller = "fast-transactions";

            public const string GetAll = Base + "/" + controller + "/all";

            public const string GetByIBAN = Base + "/" + controller + "/iban/{iban}";

            public const string GetById = Base + "/" + controller + "/{id:int}";

            public const string Post = Base + "/" + controller + "/{id:int}";

            public const string Delete = Base + "/" + controller + "/{id:int}";

        }




        public static class Loans
        { 
            public const string controller = "loans";

            public const string GetAll = Base + "/" + controller + "/all";

            public const string GetById = Base + "/" + controller + "/{id:int}";

            public const string GetByIBAN = Base + "/" + controller + "/iban/{iban}";

            public const string GetByCustomerId = Base + "/" + controller + "/customer/{id:int}";

            public const string Post = Base + "/" + controller + "/{id:int}";

            public const string Delete = Base + "/" + controller + "/{id:int}";

        }


        public static class Token
        {
            public const string controller = "token";

            public const string Refresh = Base + "/" + controller + "/refresh";

            public const string Revoke = Base + "/" + controller + "/revoke";
        }
    }
}
