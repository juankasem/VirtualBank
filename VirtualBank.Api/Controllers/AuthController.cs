using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Api.Helpers;
using VirtualBank.Core.ApiModels;
using VirtualBank.Core.ApiRequestModels;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Entities;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Api.Helpers.ErrorsHelper;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Check if the passed email is already registered 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Auth.CheckEmailExists)]
        public async Task<ActionResult<ApiResponse<CheckEmailResponse>>> CheckEmailExists(CheckEmailRequest request)
        {
            var response = new ApiResponse<CheckEmailResponse>
            {
                Data = new CheckEmailResponse()
            };

            response.Data.Exists = await CheckEmailExists(request.Email);

            return Ok(response);
        }

        /// <summary>
        /// Create New User end point
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Auth.Register)]
        public async Task<ActionResult<ApiResponse<SignupResponse>>> Register(SignupRequest request)
        {
            var response = new ApiResponse<SignupResponse>();

            if (await CheckEmailExists(request.Email))
            {
                response.AddError(ExceptionCreator.CreateBadRequestError("email already exists"));

                return response;
            }

            var appUser = new AppUser()
            {
                UserName = request.CustomerNo,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(appUser,request.Password);

            if (result.Succeeded)
            {
                response.Data = new SignupResponse()
                {
                    Email = request.Email
                };
            }
            else
            {
                //ToDo : check api status code

                return BadRequest();
            }

            return Ok(response);
        }

        /// <summary>
        /// login to account and accquire token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request)
        {
            var response = new ApiResponse<LoginResponse>();

            var user = await _userManager.FindByNameAsync(request.CustomerNo);

            if (user == null)
            {
                response.AddError(ExceptionCreator.CreateUnauthorizedError("Invalid login credentials"));
                return Unauthorized(response);
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

            if (result.Succeeded)
            {
                response.Data = new LoginResponse()
                {
                    AccessToken = _tokenService.GenerateAccessToken(await user.GetClaimsAsync(_userManager)),
                    RefreshToken = _tokenService.GenerateRefreshToken()
                };


                user.RefreshToken = response.Data.RefreshToken;
                await _userManager.UpdateAsync(user);
            }
            else
            {
                response.AddError(ExceptionCreator.CreateUnauthorizedError("Invalid login attempt"));
                return Unauthorized(response);
            }

            return Ok(response);
        }

        

        #region Private helper methods
        /// <summary>
        /// Check if the passed email is already registered 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<bool> CheckEmailExists(string email)
        {
            if (await _userManager.FindByEmailAsync(email) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        #endregion
    }
}
