﻿using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Models.Responses;

namespace VirtualBank.Core.ApiResponseModels.CashTrasactionApiResponses
{
    public class CashTransactionListResponse
    {
        public ImmutableList<CashTransaction> CashTransactions { get; }

        public int TotalCount { get; }

        public CashTransactionListResponse(ImmutableList<CashTransaction> cashTransactions, int totalCount)
        {
            CashTransactions = cashTransactions.IsEmpty ? ImmutableList<CashTransaction>.Empty : cashTransactions;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
