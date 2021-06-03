using System;

using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiResponseModels;

namespace VirtualBank.Core.Interfaces
{
    public interface IMailService
    {
        Task<Response> SendEmailAsync(string toEmail, string subject, string content, CancellationToken cancellationToken);

    }
}
