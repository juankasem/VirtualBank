using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace VirtualBank.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(List<Claim> claims);
    }
}
