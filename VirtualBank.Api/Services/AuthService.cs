using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using VirtualBank.Api.Helpers;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.AuthApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AuthApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;

namespace VirtualBank.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<AppUser> userManager,
                           SignInManager<AppUser> signInManager,
                           IConfiguration configuration,
                           IMailService mailService,
                           ITokenService tokenService
                           )
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _mailService = mailService;
            _tokenService = tokenService;
        }


        /// <summary>
        /// Register new user 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<SignupResponse>> RegisterAsync(SignupRequest request, CancellationToken cancellationToken)
        {
            var apiResponse = new ApiResponse<SignupResponse>();

            var appUser = new AppUser()
            {
                UserName = request.CustomerNo,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(appUser, request.Password);

            if (result.Succeeded)
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmationToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/confirm-email/userId={appUser.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(request.Email, "Confirm your email", $"<h1>Welcome to Authentication</h1>" +
                    $"<p>please confirm your email by <a href='{url}'>clicking here</a></p>"
                    , cancellationToken);

                apiResponse.Data = new SignupResponse()
                {
                    Email = request.Email
                };

                return apiResponse;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    apiResponse.AddError(new ErrorResponse(error.Description));
                }
            }

            return apiResponse;
        }


        /// <summary>
        /// Login user 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var apiResponse = new ApiResponse<LoginResponse>();

            var user = await _userManager.FindByNameAsync(request.CustomerNo);

            if (user == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateUnauthorizedError("Invalid login credentials"));
                return apiResponse;
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

            if (result.Succeeded)
            {
                apiResponse.Data = new LoginResponse()
                {
                    AccessToken = _tokenService.GenerateAccessToken(await user.GetClaimsAsync(_userManager)),
                    RefreshToken = _tokenService.GenerateRefreshToken()
                };

                user.RefreshToken = apiResponse.Data.RefreshToken;
                await _userManager.UpdateAsync(user);
            }
            else
            {
                apiResponse.AddError(ExceptionCreator.CreateUnauthorizedError("Invalid login attempt"));
                return apiResponse;
            }

            return apiResponse;
        }


        /// <summary>
        /// forgot password request
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> ForgotPasswordAsync(string email, CancellationToken cancellationToken)
        {
            var apiResponse = new ApiResponse<ForgotPasswordResponse>();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(user), $"user not found"));
                return apiResponse;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/reset-password?email={email}&token={validToken}";

            await _mailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instructions to reset your passwod</h1>" +
                                                    $"<p>To reset your password <a href='{url}'>Click here</a></p>", cancellationToken);


            return apiResponse;
        }



        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var apiResponse = new Response();

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(user), $"user not found"));
                return apiResponse;
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (result.Succeeded)
            {
                apiResponse.Message = "Password has been reset successfully!";
                return apiResponse;
            }

            foreach (var error in result.Errors)
            {
                apiResponse.AddError(new ErrorResponse(error.Description));
            }

            return apiResponse;
        }



        /// <summary>
        /// Check email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckEmailAsync(string email, CancellationToken cancellationToken)
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


        /// <summary>
        /// confirm email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken)
        {
            var apiResponse = new Response();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                apiResponse.AddError(ExceptionCreator.CreateNotFoundError(nameof(user), $"user not found"));
                return apiResponse;
            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            var normalizedToken = WebEncoders.Base64UrlEncode(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalizedToken);

            if (result.Succeeded)
            {
                apiResponse.Message = "Your email has been confirmed successfully!";
                return apiResponse;
            }

            foreach (var error in result.Errors)
            {
                apiResponse.AddError(new ErrorResponse(error.Description));
            }

            return apiResponse;
        }
    }
}
