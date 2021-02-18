using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace VirtualBank.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);

        string GenerateRefreshToken();

        ClaimsPrincipal GetClaimsFromExpiredToken(string token);
    }
}
