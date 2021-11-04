using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Core.ApiResponseModels.UtilityPaymentApiResponses
{
    public class UtilityPaymentListResponse
    {

        public ImmutableList<UtilityPayment> UtilityPayments { get; }

        public int TotalCount { get; }

        public UtilityPaymentListResponse(ImmutableList<UtilityPayment> utilityPayments, int totalCount)
        {
            UtilityPayments = utilityPayments.IsEmpty ? ImmutableList<UtilityPayment>.Empty : utilityPayments;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
