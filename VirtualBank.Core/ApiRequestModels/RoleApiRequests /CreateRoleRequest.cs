using System;
using System.ComponentModel.DataAnnotations;

namespace VirtualBank.Core.ApiRequestModels.RoleApiRequests
{
    public class CreateRoleRequest
    {
        [Required]
        public string RoleName { get; set; }


        public CreateRoleRequest()
        {
        }
    }
}
