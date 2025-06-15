using System.Text;
using System.Text.Json;
using FileProvider.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FileProvider.Services;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _brevoApiKey;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public EmailService(IConfiguration config)
    {
        _brevoApiKey = config["BrevoApiKey"];
        _senderEmail = config["SenderEmail"];
        _senderName = config["SenderName"];
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("api-key", _brevoApiKey);
    }

    public async Task SendEmailAsync(string recipientEmail, string fileUrl)
    {
        var emailRequest = new
        {
            sender = new { email = _senderEmail, name = _senderName },
            to = new[] { new { email = recipientEmail } },
            subject = "Your Order Report",
            htmlContent = $"<p>Your report is ready. <a href='{fileUrl}'>Download here</a></p>"
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(emailRequest), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", jsonContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to send email. Status: {response.StatusCode}. Content: {responseContent}");
                throw new Exception("Brevo failed");
            }
            response.EnsureSuccessStatusCode();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
            throw;
        }
    }
}
