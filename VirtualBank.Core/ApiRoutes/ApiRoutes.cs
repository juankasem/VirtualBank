using System;
namespace VirtualBank.Core.ApiRoutes
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;


        public static class Addresses
        {
            public const string GetAll = Base + "/address/all";

            public const string GetById = Base + "/address/{addressId:int}";

            public const string Post = Base + "/address/{addressId:int}";

            public const string Delete = Base + "/address/{addressId:int}";
        }


        public static class Administration
        {
            public const string ListRoles = Base + "/administration/roles/list";

            public const string GetUsersInRoles = Base + "/administration/roles/users";

            public const string EditUsersInRoles = Base + "/administration/roles/users";

            public const string CreateRole = Base + "/administration/roles";

        }


        public static class Auth
        {
            public const string CheckEmailExists = Base + "/auth/checkEmailExists";

            public const string Register = Base + "/auth/register";

            public const string Login = Base + "/auth/login";
        }


        public static class BankAccounts
        {
            public const string GetByCustomerId = Base + "/bank-account/customer/{customerId:int}";

            public const string GetById = Base + "/bank-account/{accountId:int}";

            public const string GetByAccountNo = Base + "/bank-account/account-no/{accountNo}";

            public const string GetByIBAN = Base + "/bank-account/iban/{iban}";

            public const string GetRecipientByIBAN = Base + "/bank-account/recipient-iban/{iban}";

            public const string Post = Base + "/bank-account/{accountId:int}";

            public const string Activate = Base + "/bank-account/activate/{accountId:int}";

            public const string Deactivate = Base + "/bank-account/deactivate/{accountId:int}";
        }


        public static class Branches
        {

            public const string GetAll = Base + "/branch/all";

            public const string GetByCityId = Base + "/branch/city/{cityId:int}";

            public const string GetById = Base + "/branch/{branchId:int}";

            public const string GetByCode = Base + "/branch/code/{code}";

            public const string Post = Base + "/branch/{branchId:int}";

            public const string Delete = Base + "/branch/{branchId:int}";
        }



        public static class Districts
        {
            public const string GetAll = Base + "/districts/all";

            public const string GetByCityId = Base + "/districts/city/{cityId:int}";

            public const string GetById = Base + "/districts/{districtId:int}";

            public const string Post = Base + "/districts/{districtId:int}";
        }


        public static class Cities
        {
            public const string GetAll = Base + "/cities/all";

            public const string GetByCountryId = Base + "/cities/country/{countryId:int}";

            public const string GetById = Base + "/cities/{cityId:int}";

            public const string Post = Base + "/cities/{cityId:int}";
        }


        public static class Countries
        {
            public const string GetAll = Base + "/countries/all";

            public const string GetById = Base + "/countries/{countryId:int}";

            public const string Post = Base + "/countries/{countryId:int}";
        }


        public static class CashTransactions
        {
            public const string GetAll = Base + "/cash-transactions/all";

            public const string GetByAccountNo = Base + "/cash-transactions/account-no/{accountNo}";

            public const string GetByIBAN = Base + "/cash-transactions/iban/{iban}";

            public const string Post = Base + "/cash-transactions";
        }


        public static class CreditCards
        {
            public const string GetAll = Base + "/credit-cards/all";

            public const string GetById = Base + "/credit-cards/{creditCardId:int}";

            public const string GetByCreditCardNo = Base + "/credit-cards/card-no/{creditCardNo";

            public const string GetByAccountNo = Base + "/credit-cards/account/{accountNo}";

            public const string Post = Base + "/credit-cards/{customerId:int}";

            public const string Activate = Base + "/credit-cards/activate/{creditCardId:int}";

            public const string Deactivate = Base + "/credit-cards/deactivate/{creditCardId:int}";
        }




        public static class Customers
        {
            public const string GetAll = Base + "/customers/all";

            public const string GetById = Base + "/customers/{customerId:int}";

            public const string GetByAccountNo = Base + "/customers/account/{accountNo}";

            public const string GetByIBAN = Base + "/customers/iban/{iban}";

            public const string Post = Base + "/customers/{customerId:int}";
        }




        public static class DebitCards
        {
            public const string GetAll = Base + "/debit-cards/all";

            public const string GetById = Base + "/debit-cards/{debitCardId:int}";

            public const string GetByDebitCardNo = Base + "debit-cards/card-no/{debitCardNo";

            public const string GetByAccountNo = Base + "/debit-cards/account/{accountNo}";

            public const string Post = Base + "/debit-cards/{customerId:int}";

            public const string Activate = Base + "/debit-cards/activate/{debitCardId:int}";

            public const string Deactivate = Base + "/debit-cards/deactivate/{debitCardId:int}";
        }



        public static class FastTransactions
        {
            public const string GetAll = Base + "/fast-transactions/all";

            public const string GetByIBAN = Base + "/fast-transactions/iban/{iban}";

            public const string GetById = Base + "/fast-transactions/{id:int}";

            public const string Post = Base + "/fast-transactions/{id:int}";

            public const string Delete = Base + "/fast-transactions/{id:int}";

        }



        public static class Token
        {
            public const string Refresh = "/token/refresh";

            public const string Revoke = "/token/revoke";
        }
  

    }
}
