using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.UtilityPaymentApiResponses
{
    public class UtilityPaymentListResponse
    {

        public ImmutableList<UtilityPaymentResponse> UtilityPayments { get; }

        public int TotalCount { get; }

        public UtilityPaymentListResponse(ImmutableList<UtilityPaymentResponse> utilityPayments, int totalCount)
        {
            UtilityPayments = utilityPayments.IsEmpty ? ImmutableList<UtilityPaymentResponse>.Empty : utilityPayments;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
