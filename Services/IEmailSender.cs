
namespace TaskScheduler.Services
{
    public interface IEmailSender
    {
        Task SendEmail(string recipent, string subject, string body);
    }
}