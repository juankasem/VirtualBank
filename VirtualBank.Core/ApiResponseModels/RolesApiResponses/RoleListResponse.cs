using System;
using System.Collections.Immutable;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.RolesApiResponses
{
    public class RoleListResponse
    {
        public ImmutableList<RoleResponse> Roles { get; }

        public int TotalCount { get; }


        public RoleListResponse(ImmutableList<RoleResponse> roles, int totalCount)
        {
            Roles = roles.IsEmpty ? ImmutableList<RoleResponse>.Empty : roles;
            TotalCount = Throw.ArgumentOutOfRangeException.IfLessThan(totalCount, 0, nameof(totalCount));
        }
    }
}
