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
        public const string getAccountsByCustomerId = "api/BankAccount/getByCustomerId/{customerId}";

        public const string getAccountByAccountNo = "api/BankAccount/getByAccountNo/{accountNo}";

        public const string getAccountByIBAN = "api/BankAccount/getByIBAN/{iban}";

        public const string getRecipientAccountByIBAN = "api/BankAccount/getRecipientByIBAN/{iban}";

        public const string postBankAccount = "api/BankAccount/post/{accountNo}";

        public const string activateBankAccount = "api/BankAccount/activate/{accountNo}";

        public const string deactivateBankAccount = "api/BankAccount/deactivate/{accountNo}";

        #endregion

        #region Branch
        public const string getAllBranches = "api/Branch/getAllBranches";

        public const string getBranchesByCityId = "api/Branch/getBranchesByCityId/{cityId}";

        public const string getBranchByCode = "api/Branch/getBranchByCode/{code}";

        public const string postBranch = "api/Branch/postBranch/{code}";

        public const string deleteBranch = "api/Branch/deleteBranch/{code}";

        #endregion


        #region Cash Transactions
        public const string getCashTransactionsByAccountNo = "api/CashTransactions/getByAccountNo/{accountNo}";

        public const string postCashTransaction = "api/CashTransactions/post";


        #endregion

        #region
        public const string getCustomerById = "api/Customer/getById/{customerId}";

        public const string getCustomerByAccountNo = "api/Customer/getByAccountNo/{accountNo}";

        public const string getCustomerByIBAN = "api/Customer/getByIBAN/{iban}";

        public const string postCustomer= "api/Customer/post/{customerId}";

        #endregion

        #region Token
        public const string refresh = "api/Token/refresh";

        public const string revoke = "api/Token/revoke";


        #endregion


    }
}
