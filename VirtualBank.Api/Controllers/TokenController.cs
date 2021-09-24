using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.Helpers;
using VirtualBank.Core.ApiRequestModels;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Entities;
using VirtualBank.Core.ApiRoutes;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;

        public TokenController(ITokenService tokenService, UserManager<AppUser> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        /// <summary>
        /// Issue a new access token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Token.Refresh)]
        public async Task<ActionResult<ApiResponse<TokenResponse>>> Refresh(TokenRequest tokenRequest)
        {
            var principal = _tokenService.GetClaimsFromExpiredToken(tokenRequest.AccessToken);
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                return BadRequest("Invalid Access Token");
            }

            if (tokenRequest.RefreshToken != user.RefreshToken)
            {
                return BadRequest("Invalid Refresh Token");
            }

            var response = new ApiResponse<TokenResponse>
            {
                Data = new TokenResponse()
            };

            var claims = new List<Claim>()
                {
                   new Claim(ClaimTypes.Name, user.UserName),
                   new Claim(ClaimTypes.Role, "")
                };

            response.Data.AccessToken = _tokenService.GenerateAccessToken(await user.GetClaimsAsync(_userManager));
            response.Data.RefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = response.Data.RefreshToken;

            await _userManager.UpdateAsync(user);

            return response;
        }

        /// <summary>
        /// delete the current refresh token of current user
        /// </summary>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Token.Revoke)]
        [Authorize]
        public async Task<Response> Revoke()
        {
            var user = await _userManager.GetUserAsync(User);
            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return new Response();
        }
    }
}
