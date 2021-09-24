using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses
{
    public class CashTransactionResponse
    {
        public CashTransaction CashTransaction { get; }

        public CashTransactionResponse(CashTransaction cashTransaction)
        {
            CashTransaction = Throw.ArgumentNullException.IfNull(cashTransaction, nameof(cashTransaction));
        }
    }
}
