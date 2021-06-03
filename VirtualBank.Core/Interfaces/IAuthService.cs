using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.AuthApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.AuthApiResponses;

namespace VirtualBank.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<SignupResponse>> RegisterAsync(SignupRequest request, CancellationToken cancellationToken);

        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

        Task<Response> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken);

        Task<Response> ForgotPasswordAsync(string email, CancellationToken cancellationToken);

        Task<Response> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);

        Task<bool> CheckEmailAsync(string email, CancellationToken cancellationToken);
    }
}
