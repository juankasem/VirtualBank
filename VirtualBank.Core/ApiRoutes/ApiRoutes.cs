using System;
namespace VirtualBank.Core.ApiRoutes
{
    public static class ApiRoutes
    {
        #region Address
        public const string getAllAddresses = "api/Address/getAl";

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
        public const string getAllBranches = "api/Branch/getAll";

        public const string getBranchesByCityId = "api/Branch/getByCityId/{cityId:int}";

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
        public const string getAllCities = "api/Cities/getAll";

        public const string getCitiesByCountryId = "api/Cities/getByCountryId";

        public const string getCityById = "api/Cities/getById/{cityId:int}";

        public const string postCity = "api/Cities/post/{cityId:int}";

        #endregion


        #region Country
        public const string getAllCountries = "api/Countries/getAll";

        public const string getCountryById = "api/Countries/getById/{countryId:int}";

        public const string postCountry = "api/Countries/post/{countryId:int}";

        #endregion


        #region Cash Transactions

        public const string getAllCashTransactions = "api/CashTransactions/getAll";

        public const string getCashTransactionsByAccountNo = "api/CashTransactions/getByAccountNo/{accountNo}";

        public const string getAccountCashTransactions = "api/CashTransactions/getByIBAN/{iban}";

        public const string postCashTransaction = "api/CashTransactions/post";

        #endregion

        #region Credit Cards
        public const string getAllCreditCards = "api/CreditCard/getAll";

        public const string getCreditCardById = "api/CreditCard/getById/{creditCardId:int}";

        public const string getCreditCardByAccountNo = "api/CreditCard/getByAccountNo/{accountNo}";

        public const string postCreditCard = "api/CreditCard/post/{customerId:int}";

        public const string activateCreditCard = "api/CreditCard/activate/{creditCardId:int}";

        public const string deactivateCreditCard = "api/CreditCard/deactivate/{creditCardId:int}";

        #endregion

        #region Customers
        public const string getAllCustomers = "api/Customer/getAll";

        public const string getCustomerById = "api/Customer/getById/{customerId:int}";

        public const string getCustomerByAccountNo = "api/Customer/getByAccountNo/{accountNo}";

        public const string getCustomerByIBAN = "api/Customer/getByIBAN/{iban}";

        public const string postCustomer= "api/Customer/post/{customerId:int}";

        #endregion


        #region Debit Cards
        public const string getAllDebitCards = "api/DebitCard/getAll";

        public const string getDebitCardById = "api/DebitCard/getById/{debitCardId:int}";

        public const string getDebitCardByAccountNo = "api/DebitCard/getByAccountNo/{accountNo}";

        public const string postDebitCard = "api/DebitCard/post/{customerId:int}";

        public const string activateDebitCard = "api/DebitCard/activate/{debitCardId:int}";

        public const string deactivateDebitCard = "api/DebitCard/deactivate/{debitCardId:int}";

        #endregion

        #region Fast Transactions
        public const string getAllFastTransactions = "api/FastTransactions/getAll";

        public const string getBankAccountFastTransactions = "api/FastTransactions/getAccountFastTransactions/{accountId:int}";

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
