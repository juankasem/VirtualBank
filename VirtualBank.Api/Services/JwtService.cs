using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Models;

namespace VirtualBank.Api.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GenerateAccessToken(List<Claim> claims)
        {
            var jwtOptions = _configuration.GetSection("JwtOptions").Get<JwtOptions>();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.k));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(jwtOptions.Lifetime),
                signingCredentials: signingCredentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return tokenString;
        }
    }
}
