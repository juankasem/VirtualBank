using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

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
