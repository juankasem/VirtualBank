using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using VirtualBank.Core.Interfaces;

namespace VirtualBank.Api.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Response> SendEmailAsync(string toEmail, string subject, string content, CancellationToken cancellationToken)
        {
            var apiKey = _configuration["SendGridAPKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("testvirtualbank.com", "Virtual Bank");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);

            return response;
        }
    }
}
