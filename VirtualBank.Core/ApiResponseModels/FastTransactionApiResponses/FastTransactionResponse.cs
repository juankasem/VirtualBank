using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses
{
    public class FastTransactionResponse
    {
        public FastTransaction FastTransaction { get; }

        public FastTransactionResponse(FastTransaction fastTransaction)
        {
            FastTransaction = Throw.ArgumentNullException.IfNull(fastTransaction, nameof(fastTransaction));
        }
    }
}
