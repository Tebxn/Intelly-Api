using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Intelly_Api.Tools

{
    public class EmailTools
    {
        private readonly IConfiguration _configuration;

        public void SendEmail(string recipient, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Remitente", _configuration.GetConnectionString("EmailAddress")));
            message.To.Add(new MailboxAddress("Destinatario", recipient));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);
                client.Authenticate(_configuration.GetConnectionString("EmailAddress"), _configuration.GetConnectionString("EmailPassword"));
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
