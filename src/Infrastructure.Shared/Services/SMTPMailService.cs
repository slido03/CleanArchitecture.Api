using CleanArchitecture.Application.Configurations;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Requests.Mail;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Shared.Services
{
    public class SMTPMailService : IMailService
    {
        private readonly MailConfiguration _config;
        private readonly ILogger<SMTPMailService> _logger;

        public SMTPMailService(IOptions<MailConfiguration> config, ILogger<SMTPMailService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var email = new MimeMessage
                {
                    Sender = new MailboxAddress(_config.DisplayName, _config.From),
                    Subject = request.Subject,
                    Body = new BodyBuilder
                    {
                        HtmlBody = request.Body
                    }.ToMessageBody()
                };
                email.From.Add(new MailboxAddress(_config.DisplayName, _config.From));
                email.To.Add(MailboxAddress.Parse(request.To));
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config.Host, _config.Port, true);
                await smtp.AuthenticateAsync(_config.UserName, _config.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}