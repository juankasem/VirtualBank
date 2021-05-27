using System;
using System.Collections.Generic;
using VirtualBank.Core.Models;

namespace VirtualBank.Core.ApiResponseModels.ClaimsApiResponses
{
    public class UserClaimResponse
    {
        public string UserId  { get; set; }
        public List<UserClaim> Claims { get; set; }

        public UserClaimResponse()
        {
            Claims = new List<UserClaim>();
        }
    }
}
