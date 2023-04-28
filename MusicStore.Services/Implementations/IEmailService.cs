namespace MusicStore.Services.Implementations;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}