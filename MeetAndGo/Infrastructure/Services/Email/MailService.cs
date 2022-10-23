using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;

namespace MeetAndGo.Infrastructure.Services.Email
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }

    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings, ILogger<MailService> logger)
        {
            _logger = logger;
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
                message.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                message.Subject = mailRequest.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = mailRequest.Body;
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, false);
                await client.AuthenticateAsync(_mailSettings.User, _mailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            catch (Exception ex)
            {
                _logger.LogError($"APP_ERROR: Could not sent e-mail. Reason: {ex.Message}");
            }
        }
    }
}
