namespace TaskScheduler.Models
{
    public class UserNotificationPreference
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Channel { get; set; } //email or sms
    }
}
