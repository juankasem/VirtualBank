using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiResponseModels.RolesApiResponses
{
    public class RoleResponse
    {
        public string RoleName { get; set; }

        public RoleResponse(string roleName)
        {
           RoleName = Throw.ArgumentNullException.IfNull(roleName, nameof(roleName));

        }
    }
}
