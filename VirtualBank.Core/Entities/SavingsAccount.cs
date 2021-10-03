using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Entities
{
    public class SavingsAccount : BankAccount
    {
        [Required]
        [Column(TypeName = "decimal(4,2)")]
        public double InterestRate { get; set; }

        [Required]
        public int AllowedNumOfTransactions { get; set; }

        [Required]
        public DateTime BlockPeriodTill { get; set; }


        public SavingsAccount(string accountNo, string iban, AccountType type, int customerId, int branchId,
                              decimal balance, decimal allowedBalanceToUse, decimal minimumAllowedBalance,
                              decimal debt, int currencyId, string createdBy,
                              DateTime createdOn, string lastModifiedBy, bool disabled, DateTime lastModifiedOn,
                              double interestRate, int allowedNumOfTransactions, DateTime blockPeriodTill)
                              :
                              base(accountNo, iban, type, customerId, branchId, balance,
                                   allowedBalanceToUse, minimumAllowedBalance, debt, currencyId,
                                   createdBy, createdOn, lastModifiedBy, lastModifiedOn, disabled)
        {
            InterestRate = interestRate;
            AllowedNumOfTransactions = allowedNumOfTransactions;
            BlockPeriodTill = blockPeriodTill;
        }
    }
}
