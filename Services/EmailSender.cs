using System.Net;
using System.Net.Mail;

namespace TaskScheduler.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(string recipient, string subject, string body)
        {
            var email = _configuration.GetValue<string>("EmailConfiguration:Email");
            var password = _configuration.GetValue<string>("EmailConfiguration:Password");
            var host = _configuration.GetValue<string>("EmailConfiguration:Host");
            var port = _configuration.GetValue<int>("EmailConfiguration:Port");

            var smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new NetworkCredential(email, password);

            var from = new MailAddress(email, "Task Scheduler");
            var to = new MailAddress(recipient);

            var message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            await smtpClient.SendMailAsync(message);

        }
    }
}
