using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace VirtualBank.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(List<Claim> claims);

        string GenerateRefreshToken();

        ClaimsPrincipal GetClaimsFromExpiredToken(string token);
    }
}
