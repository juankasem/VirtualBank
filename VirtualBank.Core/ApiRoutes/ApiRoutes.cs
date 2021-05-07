using System;
namespace VirtualBank.Core.ApiRoutes
{
    public static class ApiRoutes
    {
        #region Address
        public const string getAllAddresses = "api/Address/getAlAddresses";

        public const string getAddressById = "api/Address/getById/{addressId:int}";

        public const string postAddress = "api/Address/postAddress/{addressId:int}";

        public const string deleteAddress = "api/Address/deleteAddress/{addressId:int}";

        #endregion


        #region Auth
        public const string checkEmailExists = "api/Auth/checkEmailExists";

        public const string register = "api/Auth/register";

        public const string login = "api/Auth/login";


        #endregion

        #region Bank Account
        public const string getAccountsByCustomerId = "api/BankAccount/getByCustomerId/{customerId:int}";

        public const string getAccountById = "api/BankAccount/getById/{accountId:int}";

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


        #region District
        public const string getAllDistricts = "api/Districts/getAll";

        public const string getDistrictsByCityId = "api/Districts/getByCityId";

        public const string getDistrictById = "api/Districts/getById/{districtId:int}";

        public const string postDistrict = "api/Districts/post/{districtId:int}";

        #endregion


        #region City
        public const string getAllCities = "api/Cities/getAllCities";

        public const string getCitiesByCountryId = "api/Cities/getByCountryId";

        public const string getCityById = "api/Cities/getById/{cityId:int}";

        public const string postCity = "api/Cities/post/{cityId:int}";

        #endregion


        #region Country
        public const string getAllCountries = "api/Country/getAllCountries";

        public const string getCountryById = "api/Country/getById/{countryId:int}";

        public const string postCountry = "api/Country/post/{countryId:int}";

        #endregion


        #region Cash Transactions
        public const string getCashTransactionsByAccountNo = "api/CashTransactions/getByAccountNo/{accountNo}";

        public const string getCashTransactionsByIBAN = "api/CashTransactions/getByIBAN/{iban}";

        public const string createCashTransaction = "api/CashTransactions/create";

        #endregion

        #region Customers
        public const string getAllCustomers = "api/Customer/getAll";

        public const string getCustomerById = "api/Customer/getById/{customerId:int}";

        public const string getCustomerByAccountNo = "api/Customer/getByAccountNo/{accountNo}";

        public const string getCustomerByIBAN = "api/Customer/getByIBAN/{iban}";

        public const string postCustomer= "api/Customer/post/{customerId:int}";

        #endregion

        #region Fast Transactions
        public const string getAllFastTransactions = "api/FastTransactions/getAll";

        public const string getAccountFastTransactions = "api/FastTransactions/getAccountFastTransactions/{accountId:int}";

        public const string getFastTransactionById = "api/FastTransactions/getById/{id:int}";

        public const string postFastTransaction = "api/FastTransactions/post/{id}";

        public const string deleteFastTransaction = "api/FastTransactions/delete/{id}";


        #endregion


        #region Token
        public const string refresh = "api/Token/refresh";

        public const string revoke = "api/Token/revoke";


        #endregion


    }
}
