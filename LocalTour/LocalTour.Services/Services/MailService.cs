using LocalTour.Services.Abstract;
using LocalTour.Services.ViewModel;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
namespace LocalTour.Services.Services;

public class MailService : IMailService
{
    private readonly IConfiguration _configuration;
    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async void SendEmail(SendEmailModel model)
    {
        var _smtpUsername = _configuration["Smtp:smtpUsername"];
        var _smtpAppPassword = _configuration["Smtp:smtpAppPassword"];

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("LocalTour Notification", _smtpUsername));
            message.To.Add(new MailboxAddress("", model.To));
            message.Subject = model.Subject;

            string templatePath;
            if (model.IsApproved)
            {
                templatePath = Path.Combine(Directory.GetCurrentDirectory(), "TemplateEmail", "ApprovedEmailTemplate.html");
            }
            else
            {
                templatePath = Path.Combine(Directory.GetCurrentDirectory(), "TemplateEmail", "RejectedEmailTemplate.html");
            }

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Email template not found: {templatePath}");
            }
            
            var templateContent = await File.ReadAllTextAsync(templatePath);
            templateContent = templateContent.Replace("{{ServiceOwnerName}}", model.ServiceOwnerName);
            templateContent = templateContent.Replace("{{PlaceName}}", model.PlaceName);
            if (!model.IsApproved)
            {
                templateContent = templateContent.Replace("{{RejectionReason}}", model.RejectReason);
            }
            
            
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = templateContent };
            using (var client = new SmtpClient())
            {
               await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls); 
               await client.AuthenticateAsync(_smtpUsername, _smtpAppPassword); 
               await client.SendAsync(message);
               await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi gửi email: {ex.Message}");
            throw;
        }
    }
    
    
}