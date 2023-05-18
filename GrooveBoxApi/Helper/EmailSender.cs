using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using System.Text;
using FluentEmail.Core;
using FluentEmail.Smtp;
using FluentEmail.Razor;

namespace GrooveBoxApi.Helper;

public class EmailSender : IEmailSender
{
    private readonly IOptions<SmtpSettings> _smtpSettings;

    public EmailSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings;
    }

    public async Task SendEmailAsync(string email, string subject, string body)
    {
        var sender = new SmtpSender(() => new SmtpClient("localhost")
        {
            EnableSsl = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Port = 25,
        });

        StringBuilder template = new();
        template.AppendLine($"Dear @Model.Email");
        template.AppendLine($"<p>{body}</p>");

        Email.DefaultSender = sender;
        Email.DefaultRenderer = new RazorRenderer();

        var e = await Email
            .From(_smtpSettings.Value.FromAddress)
            .To(email)
            .Subject(subject)
            .UsingTemplate(template.ToString(), new { Email = email })
            .SendAsync();
    }
}
