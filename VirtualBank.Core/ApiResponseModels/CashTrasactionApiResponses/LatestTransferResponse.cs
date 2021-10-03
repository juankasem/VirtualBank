using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses
{
    public class LatestTransferResponse
    {
        public LatestTransfer LatestTransfer { get; }

        public LatestTransferResponse(LatestTransfer LatestTransfer)
        {
            LatestTransfer = Throw.ArgumentNullException.IfNull(LatestTransfer, nameof(LatestTransfer));
        }
    }
}
