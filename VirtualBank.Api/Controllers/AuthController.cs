using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualBank.Core.ApiModels;
using VirtualBank.Core.ApiRequestModels;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.Interfaces;
using VirtualBank.Core.Entities;
using VirtualBank.Core.ApiRoutes;
using VirtualBank.Api.Helpers.ErrorsHelper;
using System.Threading;
using VirtualBank.Core.ApiRequestModels.AuthApiRequests;
using VirtualBank.Api.ActionResults;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirtualBank.Api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthService _authService;
        private readonly IMailService _mailService;
        private readonly IActionResultMapper<AuthController> _actionResultMapper;

        public AuthController(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              IAuthService authService,
                              IMailService mailService,
                              IActionResultMapper<AuthController> actionResultMapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _authService = authService;
            _mailService = mailService;
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
        public async Task<IActionResult> Register(SignupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResponse = await _authService.RegisterAsync(request, cancellationToken);

                if (apiResponse.Success)
                {
                    await _mailService.SendEmailAsync(request.Email, "New user registration", "<p>New user" + DateTime.Now + "</p>", cancellationToken);
                    return Ok(apiResponse);
                }

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);

            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        /// <summary>
        /// login to account and accquire token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Auth.Login)]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var apiResponse = await _authService.LoginAsync(request, cancellationToken);

                if (apiResponse.Success)
                   return Ok(apiResponse);
              

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                   return NotFound(apiResponse);


                return BadRequest(apiResponse);

            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        /// <summary>
        /// forgot password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Auth.ForgotPassword)]
        public async Task<IActionResult> ForgotPassword(string email, CancellationToken cancellationToken)
        {
            var apiResponse = new Response();

            if (string.IsNullOrEmpty(email))
            {
                return NotFound(apiResponse);
            }

            try
            {
               apiResponse = await _authService.ForgotPasswordAsync(email, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);


                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        /// <summary>
        /// reset password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Auth.ResetPassword)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken)
        {

            try
            {
               var apiResponse = await _authService.ResetPasswordAsync(request, cancellationToken);

                if (apiResponse.Success)
                    return Ok(apiResponse);

                else if (apiResponse.Errors[0].Code == StatusCodes.Status404NotFound)
                    return NotFound(apiResponse);

                return BadRequest(apiResponse);
            }

            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        /// <summary>
        /// Confirm password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Auth.ConfirmEmail)]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, CancellationToken cancellationToken)
        {
            var apiResponse = new Response();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(user), $"user not found"));
                return NotFound(apiResponse);
            }

            try
            {
                apiResponse = await _authService.ConfirmEmailAsync(userId, token, cancellationToken);

                if (apiResponse.Success)
                {
                    return Ok(apiResponse);
                }

                return BadRequest(apiResponse);
            }
            catch (Exception exception)
            {
                return _actionResultMapper.Map(exception);
            }
        }


        #region Private helper methods
        /// <summary>
        /// Check if the passed email is already registered 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
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
