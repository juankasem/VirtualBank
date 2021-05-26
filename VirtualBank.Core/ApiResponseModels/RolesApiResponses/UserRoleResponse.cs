using System;
namespace VirtualBank.Core.ApiResponseModels.RolesApiResponses
{
    public class UserRoleResponse
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool HasRole { get; set; }

        public UserRoleResponse()
        {
        }
    }
}
