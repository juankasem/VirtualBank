using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtualBank.Core.Entities;

namespace VirtualBank.Api.Helpers
{
    public static class UserHelper
    {
        /// <summary>
        /// Retrieve the claims of the passed user to use them in generating tokens
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Claim>> GetClaimsAsync(this AppUser user, UserManager<AppUser> userManager)
        {
            var roles = await userManager.GetRolesAsync(user);

             var claims = new List<Claim>()
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.NameIdentifier, user.Id),
               new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
