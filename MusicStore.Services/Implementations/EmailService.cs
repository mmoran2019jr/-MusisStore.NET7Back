using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicStore.Entities;

namespace MusicStore.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IOptions<AppSettings> _options;

    public EmailService(ILogger<EmailService> logger, IOptions<AppSettings> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            if (_options.Value.SmtpConfiguration == null)
            {
                throw new SmtpException("No se ha configurado ningun valor para SMTP");
            }

            var mailMessage = new MailMessage(
                new MailAddress(_options.Value.SmtpConfiguration.UserName, _options.Value.SmtpConfiguration.FromName),
                new MailAddress(email))
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            using var smtpClient = new SmtpClient(_options.Value.SmtpConfiguration.Server, _options.Value.SmtpConfiguration.PortNumber)
            {
                Credentials = new NetworkCredential(
                    _options.Value.SmtpConfiguration.UserName, _options.Value.SmtpConfiguration.Password),
                EnableSsl = _options.Value.SmtpConfiguration.EnableSsl,
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            _logger.LogWarning(ex, "No se puede enviar el correo {message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error al enviar correo a {email} {message}", email, ex.Message);
        }
    }
}