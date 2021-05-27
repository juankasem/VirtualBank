using System;
using VirtualBank.Core.ArgumentChecks;

namespace VirtualBank.Core.ApiRequestModels.RoleApiRequests
{
    public class EditUserRoleRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }

        public EditUserRoleRequest(string userId, string userName, bool isSelected)
        {
            UserId = Throw.ArgumentNullException.IfNull(userId, nameof(userId));
            UserName = Throw.ArgumentNullException.IfNull(userName, nameof(userName));
            IsSelected = isSelected;
        }
    }
}
