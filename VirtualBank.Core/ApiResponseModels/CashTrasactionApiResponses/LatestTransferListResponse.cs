using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses
{
    public class LatestTransferListResponse
    {
        public ImmutableList<LatestTransfer> LatestTransfers { get; }

        public int TotalCount { get; }


        public LatestTransferListResponse(ImmutableList<LatestTransfer> latestTransfers, int totalCount)
        {
            LatestTransfers = latestTransfers.IsEmpty ? ImmutableList<LatestTransfer>.Empty : latestTransfers;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
