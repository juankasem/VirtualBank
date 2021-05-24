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

            public const string Delete= Base + "/branch/{branchId:int}";
        }



        public static class Districts
        {
            public const string GetAll = Base + "/districts/all";

            public const string GetByCityId = Base + "/districts/city/{cityId:int}";

            public const string GetById = Base + "/districts/{districtId:int}";

            public const string Post= Base + "/districts/{districtId:int}";
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

        


        #region Credit Cards
        public const string getAllCreditCards = "api/v1/CreditCard/getAll";

        public const string getCreditCardById = "api/v1/CreditCard/getById/{creditCardId:int}";

        public const string getCreditCardByAccountNo = "api/v1/CreditCard/getByAccountNo/{accountNo}";

        public const string postCreditCard = "api/v1/CreditCard/post/{customerId:int}";

        public const string activateCreditCard = "api/v1/CreditCard/activate/{creditCardId:int}";

        public const string deactivateCreditCard = "api/v1/CreditCard/deactivate/{creditCardId:int}";

        #endregion

        #region Customers
        public const string getAllCustomers = "api/v1/Customer/getAll";

        public const string getCustomerById = "api/v1/Customer/getById/{customerId:int}";

        public const string getCustomerByAccountNo = "api/v1/Customer/getByAccountNo/{accountNo}";

        public const string getCustomerByIBAN = "api/v1/Customer/getByIBAN/{iban}";

        public const string postCustomer= "api/v1/Customer/post/{customerId:int}";

        #endregion


        #region Debit Cards
        public const string getAllDebitCards = "api/v1/DebitCard/getAll";

        public const string getDebitCardById = "api/v1/DebitCard/getById/{debitCardId:int}";

        public const string getDebitCardByAccountNo = "api/v1/DebitCard/getByAccountNo/{accountNo}";

        public const string postDebitCard = "api/v1/DebitCard/post/{customerId:int}";

        public const string activateDebitCard = "api/v1/DebitCard/activate/{debitCardId:int}";

        public const string deactivateDebitCard = "api/v1/DebitCard/deactivate/{debitCardId:int}";

        #endregion

        #region Fast Transactions
        public const string getAllFastTransactions = "api/v1/FastTransactions/getAll";

        public const string getBankAccountFastTransactions = "api/v1/FastTransactions/getAccountFastTransactions/{accountId:int}";

        public const string getFastTransactionById = "api/v1/FastTransactions/getById/{id:int}";

        public const string postFastTransaction = "api/v1/FastTransactions/post/{id}";

        public const string deleteFastTransaction = "api/v1/FastTransactions/delete/{id}";


        #endregion


        #region Token
        public const string refresh = "api/v1/Token/refresh";

        public const string revoke = "api/v1/Token/revoke";


        #endregion


    }
}
