using System;

using System.Threading;
using System.Threading.Tasks;

namespace VirtualBank.Core.Interfaces
{
    public interface IMailService
    {
        Task<SendGrid.Response> SendEmailAsync(string toEmail, string subject, string content, CancellationToken cancellationToken);

    }
}
