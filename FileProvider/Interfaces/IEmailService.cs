namespace FileProvider.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string recipientEmail, string fileUrl);

}