using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests
{
    public class CreateCashTransactionRequest
    {
        public CashTransaction CashTransaction { get; set; }

        public CreateCashTransactionRequest(CashTransaction cashTransaction)
        {
            CashTransaction = Throw.ArgumentNullException.IfNull(cashTransaction, nameof(cashTransaction));
        }
    }
}   
