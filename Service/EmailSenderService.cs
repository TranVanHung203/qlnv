using Service.Contracts;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Service
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _config;

        public EmailSenderService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(to, subject, body, null);
        }

        public async Task SendEmailAsync(string to, string subject, string body, IEnumerable<(string FileName, byte[] Content)>? attachments = null)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"] ?? "25");
            var user = smtpSettings["User"];
            var pass = smtpSettings["Pass"];
            var from = smtpSettings["From"];

            using (var client = new SmtpClient(host, port))
            {
                client.Credentials = new NetworkCredential(user, pass);
                client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "false");

                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                if (attachments != null)
                {
                    foreach (var att in attachments)
                    {
                        var ms = new System.IO.MemoryStream(att.Content);
                        var attachment = new Attachment(ms, att.FileName);
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
