using System;
namespace VirtualBank.Core.ApiRoutes
{
    public static class ApiRoutes
    {
        #region Auth
        public const string checkEmailExists = "api/Auth/checkEmailExists";

        public const string register = "api/Auth/register";

        public const string login = "api/Auth/login";


        #endregion

        #region Bank Account
        public const string getAccountsByCustomerId = "api/BankAccount/getByCustomerId/{customerId:int}";

        public const string getAccountByAccountNo = "api/BankAccount/getByAccountNo/{accountNo}";

        public const string getAccountByIBAN = "api/BankAccount/getByIBAN/{iban}";

        public const string getRecipientAccountByIBAN = "api/BankAccount/getRecipientByIBAN/{iban}";

        public const string postBankAccount = "api/BankAccount/post/{accountId:int}";

        public const string activateBankAccount = "api/BankAccount/activate/{accountId:int}";

        public const string deactivateBankAccount = "api/BankAccount/deactivate/{accountId:int}";

        #endregion

        #region Branch
        public const string getAllBranches = "api/Branch/getAllBranches";

        public const string getBranchesByCityId = "api/Branch/getBranchesByCityId/{cityId:int}";

        public const string getBranchById = "api/Branch/getById/{branchId:int}";

        public const string getBranchByCode = "api/Branch/getByCode/{code}";

        public const string postBranch = "api/Branch/postBranch/{branchId:int}";

        public const string deleteBranch = "api/Branch/deleteBranch/{branchId:int}";

        #endregion

        #region City
        public const string getAllCities = "api/City/getAllCities";

        public const string getCitiesByCountryId = "api/City/getAllCities";

        public const string getCityById = "api/City/getById/{cityId:int}";

        public const string postCity = "api/City/postCity/{cityId:int}";

        #endregion


        #region Country
        public const string getAllCountries = "api/Country/getAllCountries";

        public const string getCountryById = "api/Country/getById/{countryId:int}";

        public const string postCountry = "api/Country/postCountry/{countryId:int}";

        #endregion


        #region Cash Transactions
        public const string getCashTransactionsByAccountNo = "api/CashTransactions/getByAccountNo/{accountNo}";

        public const string getCashTransactionsByIBAN = "api/CashTransactions/getByIBAN/{iban}";

        public const string postCashTransaction = "api/CashTransactions/post";

        #endregion

        #region
        public const string getCustomerById = "api/Customer/getById/{customerId:int}";

        public const string getCustomerByAccountNo = "api/Customer/getByAccountNo/{accountNo}";

        public const string getCustomerByIBAN = "api/Customer/getByIBAN/{iban}";

        public const string postCustomer= "api/Customer/post/{customerId:int}";

        #endregion

        #region Token
        public const string refresh = "api/Token/refresh";

        public const string revoke = "api/Token/revoke";


        #endregion


    }
}
