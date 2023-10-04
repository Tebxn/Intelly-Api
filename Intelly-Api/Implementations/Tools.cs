using Intelly_Api.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Text;
using MailKit.Net.Smtp;

namespace Intelly_Api.Implementations
{
    public class Tools : ITools
    {
        private readonly IConfiguration _configuration;

        public Tools(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            var message = new MimeMessage();
            string emailSender = _configuration["Email:SenderAddress"];
            string emailSenderPassword = _configuration["Email:SenderPassword"];
            message.From.Add(new MailboxAddress("Intelly TI Support", emailSender));//check this logic method is not sending emails
            message.To.Add(new MailboxAddress("Recipient", recipient));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);
                client.Authenticate(emailSender, emailSenderPassword);//cambiar por variables en configuracion
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }

}
