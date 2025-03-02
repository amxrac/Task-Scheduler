namespace TaskScheduler.Models
{
    public class NotificationLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int UserTaskId { get; set; }
        public string Channel { get; set; }
        public string Status { get; set; } // failed or suucess
    }
}
